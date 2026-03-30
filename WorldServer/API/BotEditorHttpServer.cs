using Common;
using FrameWork;
using GameData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Objects;
using WorldItem = WorldServer.World.Objects.Item;

namespace WorldServer.API
{
    public sealed class BotEditorHttpServer
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly HttpListener _listener = new HttpListener();
        private readonly string _prefix;
        private Thread _listenerThread;
        private volatile bool _running;

        public BotEditorHttpServer(string address, int port)
        {
            _prefix = $"http://{address}:{port}/";
        }

        public void Start()
        {
            if (_running)
                return;

            _listener.Prefixes.Add(_prefix);
            _listener.Start();
            _running = true;

            _listenerThread = new Thread(ListenLoop)
            {
                IsBackground = true,
                Name = "BotEditorHttpServer"
            };
            _listenerThread.Start();

            Log.Success("BotEditorAPI", "Bot editor API started " + _prefix);
        }

        public void Stop()
        {
            _running = false;

            try
            {
                _listener.Stop();
                _listener.Close();
            }
            catch (Exception)
            {
            }
        }

        private void ListenLoop()
        {
            while (_running)
            {
                HttpListenerContext context;
                try
                {
                    context = _listener.GetContext();
                }
                catch (HttpListenerException)
                {
                    if (!_running)
                        return;

                    continue;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                ThreadPool.QueueUserWorkItem(HandleContext, context);
            }
        }

        private void HandleContext(object state)
        {
            HttpListenerContext context = (HttpListenerContext)state;
            HttpListenerResponse response = context.Response;

            try
            {
                AddCorsHeaders(response);

                if (string.Equals(context.Request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    return;
                }

                string[] segments = context.Request.Url.AbsolutePath
                    .Trim('/')
                    .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length < 3
                    || !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase)
                    || !segments[1].Equals("bot-editor", StringComparison.OrdinalIgnoreCase))
                {
                    WriteError(response, HttpStatusCode.NotFound, "Route not found.");
                    return;
                }

                if (segments[2].Equals("health", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteError(response, HttpStatusCode.MethodNotAllowed, "Only GET is supported for this route.");
                        return;
                    }

                    WriteJson(response, HttpStatusCode.OK, new HealthResponse
                    {
                        Ok = true,
                        BotCount = BotManager.Instance.GetAllBotCharacters().Count,
                        LoadedBotCount = BotManager.Instance.GetLoadedBots().Count
                    });
                    return;
                }

                if (!segments[2].Equals("bots", StringComparison.OrdinalIgnoreCase))
                {
                    WriteError(response, HttpStatusCode.NotFound, "Route not found.");
                    return;
                }

                if (segments.Length == 3)
                {
                    if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteError(response, HttpStatusCode.MethodNotAllowed, "Only GET is supported for this route.");
                        return;
                    }

                    HandleGetBots(response);
                    return;
                }

                if (!uint.TryParse(segments[3], out uint characterId))
                {
                    WriteError(response, HttpStatusCode.BadRequest, "Invalid character id.");
                    return;
                }

                if (!TryResolveBot(characterId, out ResolvedBot bot, out string resolveError))
                {
                    WriteError(response, HttpStatusCode.NotFound, resolveError);
                    return;
                }

                if (segments.Length == 4)
                {
                    if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteError(response, HttpStatusCode.MethodNotAllowed, "Only GET is supported for this route.");
                        return;
                    }

                    WriteJson(response, HttpStatusCode.OK, BuildBotSheet(bot));
                    return;
                }

                if (segments.Length == 5 && segments[4].Equals("gear", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(context.Request.HttpMethod, "PUT", StringComparison.OrdinalIgnoreCase))
                    {
                        HandlePutGear(context.Request, response, bot);
                        return;
                    }

                    if (string.Equals(context.Request.HttpMethod, "DELETE", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleDeleteGear(context.Request, response, bot);
                        return;
                    }

                    WriteError(response, HttpStatusCode.MethodNotAllowed, "Only PUT and DELETE are supported for this route.");
                    return;
                }

                if (segments.Length == 5 && segments[4].Equals("items", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteError(response, HttpStatusCode.MethodNotAllowed, "Only GET is supported for this route.");
                        return;
                    }

                    HandleItemSearch(context.Request, response, bot);
                    return;
                }

                WriteError(response, HttpStatusCode.NotFound, "Route not found.");
            }
            catch (Exception e)
            {
                Log.Error("BotEditorAPI", e.ToString());
                WriteError(response, HttpStatusCode.InternalServerError, "Unhandled bot editor API error.", e.Message);
            }
            finally
            {
                try
                {
                    response.OutputStream.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void HandleGetBots(HttpListenerResponse response)
        {
            List<BotSummaryResponse> bots = BotManager.Instance.GetAllBotCharacters()
                .Select(BuildBotSummary)
                .OrderBy(bot => bot.Name)
                .ToList();

            WriteJson(response, HttpStatusCode.OK, bots);
        }

        private static void HandlePutGear(HttpListenerRequest request, HttpListenerResponse response, ResolvedBot bot)
        {
            UpdateBotGearRequest updateRequest = ReadRequestBody<UpdateBotGearRequest>(request);
            if (updateRequest?.Slots == null)
            {
                WriteError(response, HttpStatusCode.BadRequest, "Request body must contain a slots array.");
                return;
            }

            Dictionary<ushort, uint> overrideEntries = updateRequest.ReplaceOverrides
                ? new Dictionary<ushort, uint>()
                : BotGearOverrideService.GetOverrideEntries(bot.Character.CharacterId);

            foreach (GearSlotUpdate slot in updateRequest.Slots)
            {
                if (slot == null)
                    continue;

                if (!BotLoadoutManager.IsManagedEquipmentSlot(slot.SlotId))
                {
                    WriteError(response, HttpStatusCode.BadRequest, $"Slot {slot.SlotId} is not a managed bot equipment slot.");
                    return;
                }

                if (!slot.ItemEntry.HasValue || slot.ItemEntry.Value == 0)
                    overrideEntries.Remove(slot.SlotId);
                else
                    overrideEntries[slot.SlotId] = slot.ItemEntry.Value;
            }

            if (!TryValidateOverrideEntries(bot, overrideEntries, out string validationError))
            {
                WriteError(response, HttpStatusCode.BadRequest, validationError);
                return;
            }

            BotGearOverrideService.ReplaceOverrides(bot.Character.CharacterId, overrideEntries);
            if (updateRequest.Reapply && bot.LoadedPlayer != null)
                BotManager.Instance.TryReapplyBotLoadout(bot.Character.CharacterId);

            WriteJson(response, HttpStatusCode.OK, BuildBotSheet(RefreshResolvedBot(bot)));
        }

        private static void HandleDeleteGear(HttpListenerRequest request, HttpListenerResponse response, ResolvedBot bot)
        {
            bool reapply = !string.Equals(request.QueryString["reapply"], "false", StringComparison.OrdinalIgnoreCase);

            BotGearOverrideService.RemoveOverrides(bot.Character.CharacterId);
            if (reapply && bot.LoadedPlayer != null)
                BotManager.Instance.TryReapplyBotLoadout(bot.Character.CharacterId);

            WriteJson(response, HttpStatusCode.OK, BuildBotSheet(RefreshResolvedBot(bot)));
        }

        private static void HandleItemSearch(HttpListenerRequest request, HttpListenerResponse response, ResolvedBot bot)
        {
            string query = request.QueryString["q"]?.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                WriteError(response, HttpStatusCode.BadRequest, "Query parameter 'q' is required.");
                return;
            }

            if (!ushort.TryParse(request.QueryString["slotId"], out ushort slotId))
            {
                WriteError(response, HttpStatusCode.BadRequest, "Query parameter 'slotId' is required.");
                return;
            }

            if (!BotLoadoutManager.IsManagedEquipmentSlot(slotId))
            {
                WriteError(response, HttpStatusCode.BadRequest, $"Slot {slotId} is not a managed bot equipment slot.");
                return;
            }

            int limit = 50;
            if (int.TryParse(request.QueryString["limit"], out int parsedLimit))
                limit = Math.Max(1, Math.Min(200, parsedLimit));

            HashSet<uint> uniqueEquipped = BuildUniqueSetExcludingSlot(bot, slotId);
            bool exactEntry = uint.TryParse(query, out uint entryQuery);

            List<ItemSearchResultResponse> results = ItemService._Item_Info.Values
                .Where(item => MatchesItemQuery(item, query, exactEntry, entryQuery))
                .Where(item => BotLoadoutManager.CanUseItemInLoadout(bot.Character.CareerLine, bot.Character.Race, bot.Value.Skills, bot.Value.Level, bot.Value.RenownRank, slotId, item, uniqueEquipped))
                .OrderBy(item => exactEntry && item.Entry == entryQuery ? 0 : 1)
                .ThenBy(item => item.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenByDescending(item => item.ObjectLevel)
                .ThenBy(item => item.Name)
                .Take(limit)
                .Select(BuildItemSearchResult)
                .ToList();

            WriteJson(response, HttpStatusCode.OK, results);
        }

        private static ResolvedBot RefreshResolvedBot(ResolvedBot bot)
        {
            TryResolveBot(bot.Character.CharacterId, out ResolvedBot refreshedBot, out _);
            return refreshedBot ?? bot;
        }

        private static bool TryResolveBot(uint characterId, out ResolvedBot bot, out string error)
        {
            bot = null;
            error = "Bot not found.";

            Character character = CharMgr.GetCharacter(characterId, false);
            if (character == null)
                return false;

            if (!character.Name.StartsWith("Bot_", StringComparison.OrdinalIgnoreCase))
            {
                error = "Character is not a bot.";
                return false;
            }

            if (character.Value == null)
                character.Value = CharMgr.Database.SelectObject<Character_value>($"CharacterId={character.CharacterId}");

            if (character.Value == null)
            {
                error = "Bot character value record is missing.";
                return false;
            }

            BotRole role = BotRole.MeleeDPS;
            if (!BotManager.TryGetRoleFromBotName(character.Name, out role))
            {
                error = "Unable to determine bot role from character name.";
                return false;
            }

            Player loadedPlayer = BotManager.Instance.GetLoadedBot(characterId);
            if (loadedPlayer != null)
                role = loadedPlayer.Role;

            bot = new ResolvedBot
            {
                Character = character,
                Value = character.Value,
                LoadedPlayer = loadedPlayer,
                Role = role,
                Tier = ResolveBotTier(character.Value.Level, character.Value.RenownRank)
            };

            return true;
        }

        private static BotSummaryResponse BuildBotSummary(Character character)
        {
            Character_value value = character.Value;
            if (value == null)
                value = CharMgr.Database.SelectObject<Character_value>($"CharacterId={character.CharacterId}");

            Player loadedPlayer = BotManager.Instance.GetLoadedBot(character.CharacterId);
            BotRole role = BotRole.MeleeDPS;
            if (loadedPlayer != null)
                role = loadedPlayer.Role;
            else
                BotManager.TryGetRoleFromBotName(character.Name, out role);

            return new BotSummaryResponse
            {
                CharacterId = character.CharacterId,
                Name = character.Name,
                GroupPrefix = BotManager.GetGroupPrefixFromBotName(character.Name),
                Loaded = loadedPlayer != null,
                Role = FormatEnumName(role.ToString()),
                CareerLine = character.CareerLine,
                CareerName = FormatEnumName(((CareerLine)character.CareerLine).ToString()),
                Realm = character.Realm,
                RealmName = FormatEnumName(((Realms)character.Realm).ToString()),
                Level = value?.Level ?? 0,
                RenownRank = value?.RenownRank ?? 0,
                ZoneId = loadedPlayer?.Zone?.ZoneId ?? value?.ZoneId ?? 0,
                HasGearOverrides = BotGearOverrideService.HasOverrides(character.CharacterId)
            };
        }

        private static BotSheetResponse BuildBotSheet(ResolvedBot bot)
        {
            Dictionary<ushort, uint> templateLoadout = BotLoadoutManager.GetBaseLoadout(bot.Character.CareerLine, bot.Tier, bot.Role)?.SlotItems
                .ToDictionary(entry => entry.Key, entry => entry.Value)
                ?? new Dictionary<ushort, uint>();

            Dictionary<ushort, uint> overrideEntries = BotGearOverrideService.GetOverrideEntries(bot.Character.CharacterId);
            Dictionary<ushort, uint> effectiveLoadout = BotLoadoutManager.GetLoadout(
                bot.Character.CharacterId,
                bot.Character.CareerLine,
                bot.Character.Race,
                bot.Value.Skills,
                bot.Value.Level,
                bot.Value.RenownRank,
                bot.Tier,
                bot.Role)?.SlotItems.ToDictionary(entry => entry.Key, entry => entry.Value)
                ?? new Dictionary<ushort, uint>();

            return new BotSheetResponse
            {
                CharacterId = bot.Character.CharacterId,
                Name = bot.Character.Name,
                GroupPrefix = BotManager.GetGroupPrefixFromBotName(bot.Character.Name),
                Loaded = bot.LoadedPlayer != null,
                Role = FormatEnumName(bot.Role.ToString()),
                CareerLine = bot.Character.CareerLine,
                CareerName = FormatEnumName(((CareerLine)bot.Character.CareerLine).ToString()),
                Realm = bot.Character.Realm,
                RealmName = FormatEnumName(((Realms)bot.Character.Realm).ToString()),
                Level = bot.Value.Level,
                RenownRank = bot.Value.RenownRank,
                ZoneId = bot.LoadedPlayer?.Zone?.ZoneId ?? bot.Value.ZoneId,
                TemplateGear = BuildGearSlots(templateLoadout),
                CustomGearOverrides = BuildGearSlots(overrideEntries),
                EffectiveLoadout = BuildGearSlots(effectiveLoadout),
                EquippedGear = BuildEquippedGear(bot)
            };
        }

        private static List<GearSlotResponse> BuildEquippedGear(ResolvedBot bot)
        {
            List<GearSlotResponse> gear = new List<GearSlotResponse>();

            if (bot.LoadedPlayer?.ItmInterface != null && bot.LoadedPlayer.ItmInterface.IsLoad)
            {
                foreach (ushort slotId in BotLoadoutManager.ManagedEquipmentSlots.OrderBy(slot => slot))
                {
                    WorldItem item = bot.LoadedPlayer.ItmInterface.GetItemInSlot(slotId);
                    if (item?.Info == null)
                        continue;

                    gear.Add(new GearSlotResponse
                    {
                        SlotId = slotId,
                        SlotName = GetSlotName(slotId),
                        ItemEntry = item.Info.Entry,
                        ItemName = item.Info.Name,
                        ModelId = item.Info.ModelId,
                        ObjectLevel = item.Info.ObjectLevel,
                        MinRank = item.Info.MinRank,
                        MinRenown = item.Info.MinRenown,
                        Rarity = item.Info.Rarity,
                        PrimaryDye = item.GetPrimaryDye(),
                        SecondaryDye = item.GetSecondaryDye()
                    });
                }

                return gear;
            }

            IList<CharacterItem> persistedItems = CharMgr.Database.SelectObjects<CharacterItem>($"CharacterId={bot.Character.CharacterId}") ?? new List<CharacterItem>();
            foreach (CharacterItem item in persistedItems
                .Where(item => item != null && BotLoadoutManager.IsManagedEquipmentSlot(item.SlotId))
                .OrderBy(item => item.SlotId))
            {
                Item_Info info = ItemService.GetItem_Info(item.Entry);
                if (info == null)
                    continue;

                gear.Add(new GearSlotResponse
                {
                    SlotId = item.SlotId,
                    SlotName = GetSlotName(item.SlotId),
                    ItemEntry = item.Entry,
                    ItemName = info.Name,
                    ModelId = info.ModelId,
                    ObjectLevel = info.ObjectLevel,
                    MinRank = info.MinRank,
                    MinRenown = info.MinRenown,
                    Rarity = info.Rarity,
                    PrimaryDye = item.PrimaryDye,
                    SecondaryDye = item.SecondaryDye
                });
            }

            return gear;
        }

        private static List<GearSlotResponse> BuildGearSlots(IDictionary<ushort, uint> entries)
        {
            List<GearSlotResponse> gear = new List<GearSlotResponse>();
            foreach (KeyValuePair<ushort, uint> entry in entries.OrderBy(item => item.Key))
            {
                Item_Info info = ItemService.GetItem_Info(entry.Value);
                if (info == null)
                    continue;

                gear.Add(new GearSlotResponse
                {
                    SlotId = entry.Key,
                    SlotName = GetSlotName(entry.Key),
                    ItemEntry = info.Entry,
                    ItemName = info.Name,
                    ModelId = info.ModelId,
                    ObjectLevel = info.ObjectLevel,
                    MinRank = info.MinRank,
                    MinRenown = info.MinRenown,
                    Rarity = info.Rarity
                });
            }

            return gear;
        }

        private static bool TryValidateOverrideEntries(ResolvedBot bot, IDictionary<ushort, uint> overrideEntries, out string error)
        {
            error = null;

            LoadoutValidationContext validationContext = new LoadoutValidationContext(
                bot.Character.CareerLine,
                bot.Character.Race,
                bot.Value.Skills,
                bot.Value.Level,
                bot.Value.RenownRank,
                BotLoadoutManager.GetBaseLoadout(bot.Character.CareerLine, bot.Tier, bot.Role));

            foreach (KeyValuePair<ushort, uint> overrideEntry in overrideEntries.OrderBy(entry => entry.Key))
            {
                if (!validationContext.TryApplyOverride(overrideEntry.Key, overrideEntry.Value, out error))
                    return false;
            }

            return true;
        }

        private static HashSet<uint> BuildUniqueSetExcludingSlot(ResolvedBot bot, ushort excludedSlotId)
        {
            HashSet<uint> uniqueEntries = new HashSet<uint>();
            Dictionary<ushort, uint> effectiveLoadout = BotLoadoutManager.GetLoadout(
                bot.Character.CharacterId,
                bot.Character.CareerLine,
                bot.Character.Race,
                bot.Value.Skills,
                bot.Value.Level,
                bot.Value.RenownRank,
                bot.Tier,
                bot.Role)?.SlotItems
                .ToDictionary(entry => entry.Key, entry => entry.Value)
                ?? new Dictionary<ushort, uint>();

            foreach (KeyValuePair<ushort, uint> entry in effectiveLoadout)
            {
                if (entry.Key == excludedSlotId)
                    continue;

                Item_Info info = ItemService.GetItem_Info(entry.Value);
                if (info?.UniqueEquiped == 1)
                    uniqueEntries.Add(entry.Value);
            }

            return uniqueEntries;
        }

        private static ItemSearchResultResponse BuildItemSearchResult(Item_Info item)
        {
            return new ItemSearchResultResponse
            {
                ItemEntry = item.Entry,
                ItemName = item.Name,
                ModelId = item.ModelId,
                SlotId = item.SlotId,
                SlotName = GetSlotName(item.SlotId),
                ObjectLevel = item.ObjectLevel,
                MinRank = item.MinRank,
                MinRenown = item.MinRenown,
                Rarity = item.Rarity
            };
        }

        private static bool MatchesItemQuery(Item_Info item, string query, bool exactEntry, uint entryQuery)
        {
            if (item == null)
                return false;

            if (exactEntry && item.Entry == entryQuery)
                return true;

            return item.Name?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static BotLoadoutManager.BotTier ResolveBotTier(byte level, byte renownRank)
        {
            if (level <= 11)
                return BotLoadoutManager.BotTier.T1;
            if (level <= 21)
                return BotLoadoutManager.BotTier.T2;
            if (level <= 31)
                return BotLoadoutManager.BotTier.T3;

            if (renownRank >= 100)
                return BotLoadoutManager.BotTier.T4_RR100;
            if (renownRank >= 90)
                return BotLoadoutManager.BotTier.T4_RR90;
            if (renownRank >= 80)
                return BotLoadoutManager.BotTier.T4_RR80;
            if (renownRank >= 70)
                return BotLoadoutManager.BotTier.T4_RR70;

            return BotLoadoutManager.BotTier.T4_RR40;
        }

        private static string GetSlotName(ushort slotId)
        {
            return FormatEnumName(((EquipSlot)slotId).ToString());
        }

        private static string FormatEnumName(string value)
        {
            return value.Replace('_', ' ');
        }

        private static T ReadRequestBody<T>(HttpListenerRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
            {
                string body = reader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(body))
                    return default(T);

                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        private static void AddCorsHeaders(HttpListenerResponse response)
        {
            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Headers["Access-Control-Allow-Methods"] = "GET,PUT,DELETE,OPTIONS";
            response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
        }

        private static void WriteError(HttpListenerResponse response, HttpStatusCode statusCode, string error, string detail = null)
        {
            WriteJson(response, statusCode, new ErrorResponse
            {
                Error = error,
                Detail = detail
            });
        }

        private static void WriteJson(HttpListenerResponse response, HttpStatusCode statusCode, object payload)
        {
            string json = JsonConvert.SerializeObject(payload, JsonSettings);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.LongLength;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private sealed class ResolvedBot
        {
            public Character Character { get; set; }
            public Character_value Value { get; set; }
            public Player LoadedPlayer { get; set; }
            public BotRole Role { get; set; }
            public BotLoadoutManager.BotTier Tier { get; set; }
        }

        private sealed class LoadoutValidationContext
        {
            private readonly byte _careerLine;
            private readonly byte _race;
            private readonly long _skills;
            private readonly byte _level;
            private readonly byte _renownRank;
            private readonly Dictionary<ushort, uint> _effectiveEntries;
            private readonly HashSet<uint> _uniqueEntries;

            public LoadoutValidationContext(byte careerLine, byte race, long skills, byte level, byte renownRank, BotLoadoutManager.Loadout baseLoadout)
            {
                _careerLine = careerLine;
                _race = race;
                _skills = skills;
                _level = level;
                _renownRank = renownRank;
                _effectiveEntries = baseLoadout?.SlotItems.ToDictionary(entry => entry.Key, entry => entry.Value) ?? new Dictionary<ushort, uint>();
                _uniqueEntries = new HashSet<uint>(_effectiveEntries.Values
                    .Select(ItemService.GetItem_Info)
                    .Where(info => info?.UniqueEquiped == 1)
                    .Select(info => info.Entry));
            }

            public bool TryApplyOverride(ushort slotId, uint itemEntry, out string error)
            {
                error = null;

                if (!BotLoadoutManager.IsManagedEquipmentSlot(slotId))
                {
                    error = $"Slot {slotId} is not a managed bot equipment slot.";
                    return false;
                }

                Item_Info previousInfo = null;
                if (_effectiveEntries.TryGetValue(slotId, out uint previousEntry))
                {
                    previousInfo = ItemService.GetItem_Info(previousEntry);
                    if (previousInfo?.UniqueEquiped == 1)
                        _uniqueEntries.Remove(previousEntry);
                }

                Item_Info itemInfo = ItemService.GetItem_Info(itemEntry);
                if (!BotLoadoutManager.CanUseItemInLoadout(_careerLine, _race, _skills, _level, _renownRank, slotId, itemInfo, _uniqueEntries))
                {
                    if (previousInfo?.UniqueEquiped == 1)
                        _uniqueEntries.Add(previousInfo.Entry);

                    error = $"Item {itemEntry} is not valid for slot {slotId} on this bot.";
                    return false;
                }

                _effectiveEntries[slotId] = itemEntry;
                if (itemInfo.UniqueEquiped == 1)
                    _uniqueEntries.Add(itemInfo.Entry);

                return true;
            }
        }

        private sealed class ErrorResponse
        {
            public string Error { get; set; }
            public string Detail { get; set; }
        }

        private sealed class HealthResponse
        {
            public bool Ok { get; set; }
            public int BotCount { get; set; }
            public int LoadedBotCount { get; set; }
        }

        private sealed class BotSummaryResponse
        {
            public uint CharacterId { get; set; }
            public string Name { get; set; }
            public string GroupPrefix { get; set; }
            public bool Loaded { get; set; }
            public string Role { get; set; }
            public byte CareerLine { get; set; }
            public string CareerName { get; set; }
            public byte Realm { get; set; }
            public string RealmName { get; set; }
            public byte Level { get; set; }
            public byte RenownRank { get; set; }
            public ushort ZoneId { get; set; }
            public bool HasGearOverrides { get; set; }
        }

        private sealed class BotSheetResponse
        {
            public uint CharacterId { get; set; }
            public string Name { get; set; }
            public string GroupPrefix { get; set; }
            public bool Loaded { get; set; }
            public string Role { get; set; }
            public byte CareerLine { get; set; }
            public string CareerName { get; set; }
            public byte Realm { get; set; }
            public string RealmName { get; set; }
            public byte Level { get; set; }
            public byte RenownRank { get; set; }
            public ushort ZoneId { get; set; }
            public List<GearSlotResponse> TemplateGear { get; set; }
            public List<GearSlotResponse> CustomGearOverrides { get; set; }
            public List<GearSlotResponse> EffectiveLoadout { get; set; }
            public List<GearSlotResponse> EquippedGear { get; set; }
        }

        private sealed class GearSlotResponse
        {
            public ushort SlotId { get; set; }
            public string SlotName { get; set; }
            public uint ItemEntry { get; set; }
            public string ItemName { get; set; }
            public uint ModelId { get; set; }
            public byte ObjectLevel { get; set; }
            public byte MinRank { get; set; }
            public byte MinRenown { get; set; }
            public byte Rarity { get; set; }
            public ushort PrimaryDye { get; set; }
            public ushort SecondaryDye { get; set; }
        }

        private sealed class UpdateBotGearRequest
        {
            public bool ReplaceOverrides { get; set; } = true;
            public bool Reapply { get; set; } = true;
            public List<GearSlotUpdate> Slots { get; set; }
        }

        private sealed class GearSlotUpdate
        {
            public ushort SlotId { get; set; }
            public uint? ItemEntry { get; set; }
        }

        private sealed class ItemSearchResultResponse
        {
            public uint ItemEntry { get; set; }
            public string ItemName { get; set; }
            public uint ModelId { get; set; }
            public ushort SlotId { get; set; }
            public string SlotName { get; set; }
            public byte ObjectLevel { get; set; }
            public byte MinRank { get; set; }
            public byte MinRenown { get; set; }
            public byte Rarity { get; set; }
        }
    }
}
