using Common;
using FrameWork;
using System.Collections.Generic;
using System.Linq;

namespace WorldServer.Managers
{
    public static class BotGearOverrideService
    {
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<uint, Dictionary<ushort, BotGearOverride>> OverrideCache = new Dictionary<uint, Dictionary<ushort, BotGearOverride>>();

        public static Dictionary<ushort, uint> GetOverrideEntries(uint characterId)
        {
            lock (SyncRoot)
            {
                Dictionary<ushort, BotGearOverride> overrides = GetOrLoadOverridesUnsafe(characterId);
                return overrides.ToDictionary(entry => entry.Key, entry => entry.Value.ItemEntry);
            }
        }

        public static bool HasOverrides(uint characterId)
        {
            lock (SyncRoot)
                return GetOrLoadOverridesUnsafe(characterId).Count > 0;
        }

        public static void ReplaceOverrides(uint characterId, IDictionary<ushort, uint> overrides)
        {
            lock (SyncRoot)
            {
                Dictionary<ushort, BotGearOverride> existingOverrides = GetOrLoadOverridesUnsafe(characterId);
                foreach (BotGearOverride existing in existingOverrides.Values.ToList())
                    CharMgr.Database.DeleteObject(existing);

                Dictionary<ushort, BotGearOverride> replacement = new Dictionary<ushort, BotGearOverride>();
                if (overrides != null)
                {
                    foreach (KeyValuePair<ushort, uint> entry in overrides)
                    {
                        if (entry.Value == 0)
                            continue;

                        BotGearOverride gearOverride = new BotGearOverride
                        {
                            CharacterId = characterId,
                            SlotId = entry.Key,
                            ItemEntry = entry.Value
                        };

                        CharMgr.Database.AddObject(gearOverride);
                        replacement[entry.Key] = gearOverride;
                    }
                }

                OverrideCache[characterId] = replacement;
                CharMgr.Database.ForceSave();
            }
        }

        public static void RemoveOverrides(uint characterId)
        {
            ReplaceOverrides(characterId, null);
        }

        private static Dictionary<ushort, BotGearOverride> GetOrLoadOverridesUnsafe(uint characterId)
        {
            if (OverrideCache.TryGetValue(characterId, out Dictionary<ushort, BotGearOverride> overrides))
                return overrides;

            IList<BotGearOverride> loadedOverrides = CharMgr.Database.SelectObjects<BotGearOverride>($"CharacterId={characterId}") ?? new List<BotGearOverride>();
            overrides = loadedOverrides
                .Where(entry => entry != null)
                .GroupBy(entry => entry.SlotId)
                .ToDictionary(group => group.Key, group => group.Last());

            OverrideCache[characterId] = overrides;
            return overrides;
        }
    }
}
