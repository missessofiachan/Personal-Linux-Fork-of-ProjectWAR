# ProjectWAR Status

ProjectWAR is a C# private server emulator for Warhammer Online: Age of Reckoning. It targets .NET 4.8, x64. The solution is `WarEmulator.sln`.

## Built Projects

All of the following produce binaries under `bin/Debug/` and `bin/Release/`:

| Project | Output |
|---------|--------|
| WorldServer | `WorldServer.exe` — main game server |
| AccountCacher | `AccountCacher.exe` — auth/account service |
| LauncherServer | `LauncherServer.exe` — patch/launcher auth |
| LobbyServer | `LobbyServer.exe` — lobby service |
| Launcher | `Launcher/Launcher.exe` — game launcher client |
| ServerLauncher | `ServerLauncher.exe` — WinForms server control panel |
| LosBuilder | `LosBuilder.exe` — LOS generation and inspection tool |
| ClientDataMatrix | `ClientDataMatrix.exe` — WAR client data analysis tool (GUI + CLI) |

The C++ runtime extension `WarZone64.dll` ships prebuilt in `WorldServer/`.

Build command:
```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe' WarEmulator.sln /p:Platform=x64 /p:Configuration=Release /t:Build /m /nologo /verbosity:minimal
```

## LOS Generation Status

Native LOS generation is implemented in `LosBuilder`. All four structural parity gaps vs. the shipped `bin/Release/los/*.bin` files are resolved for zone 280:

- Region offsets: match (figleaf.db cell size is 8192 units, shift is `<< 13`)
- Terrain height orientation: match (no X-axis flip)
- Holemap behavior: match (`0x0` when `holemap.pcx` absent)
- Collision geometry: match (invisible NiTriShape nodes inside fixture NIFs only; no separate `_collision.nif` files exist)

Zone 280 triangle hash matches shipped exactly (`0xBAAEA5D12555DEB1`).

Remaining gaps:
- Vertex position precision: ~16-unit max Y error from NIF world-matrix accumulation. Topology is correct.
- Water generation: zone 280 has no `water.xml` in current extracted data; file-size difference vs. shipped is exactly one water chunk (136 bytes).
- Multi-zone coverage: only zone 280 has all required source files in the current extraction.

See `docs/los/occ-re-notes.md` for OCC format documentation and full per-chunk comparison details.

## ClientDataMatrix Tool

`ClientDataMatrix` is a read-only analysis tool that ingests extracted WAR client files and builds a provenance-tracked graph of ability, effect, and identity data. It produces per-ability reports, conflict ledgers, coverage summaries, and operation schema ledgers.

Source files it reads are all under `C:\Users\Admin\Downloads\myps`:
- `data/gamedata/abilities.csv`, `effects.csv`, `pregame_chars.xml`
- `data/strings/english/abilitynames.txt`, `abilitydesc.txt`, `abilityeffect.txt`, `careernames_m.txt`, `careerlines_m.txt`, `racenames_m.txt`
- `data/bin/abilityexport.bin`, `abilitycomponentexport.bin`, `abilityrequirementexport.bin`

Reports are generated at runtime to a local output directory. They are not committed to the repo.

Tool usage: `docs/client-data-matrix-usage.md`.

### Component Field Decode Status (as of commit 6fb7cfa3, 2026-03-28)

All 18,526 component records across all operation types have been fully decoded. **Unknown = 0, Structural = 0.**

Every field in `abilitycomponentexport.bin` is at Confirmed or Inferred confidence:
- **ExtData Val1–Val4**: universal cross-op decode (ApplicationTarget, ComponentOperation type, ApplicationProfile, LayoutTag)
- **Values[0–7]**: per-operation semantics for all 40+ operation types including unnamed ops (29, 30, 32, 40, 41, 43, 47, 51)
- **Multipliers[0–7]**: scaling percentages per operation
- **FlagsRaw**: CrowdControlTypes bitmasks, bit-field flags, sequential enums per operation
- **Value08**: universal binary flag; **Value15**: universal CC gate (CrowdControlTypes bits, mask 0x8FF)

Confirmed resolves this session:
- `DAMAGE Value[1]` → **`MaxCounter`** (counter cap/tick limit) — from decompiled server `DAMAGE.cs`
- `DAMAGE FlagsRaw` → **`DamageFlag`** enum (NONE=0, UNMITIGATABLE=1) — same source

All 558 requirement rows decoded (Val1=AbilitySourceType, Val2=AbilityOperation, Val3=AbilityCondition, Val4=AbilityLogicOperator — from decompiled client `AbilityExport.cs`).

### Coverage Status (as of commit 6fb7cfa3)

| Status | Count | Notes |
|--------|------:|-------|
| MappedWithRequirements | 390 | Fully mapped with requirement links |
| Mapped | 1,984 | Fully mapped |
| Partial | 11,635 | Have BIN or CSV but missing some pieces |
| StringsOnly | 1,029 | Named in strings but no BIN/CSV/effect/component |
| BlankSlot | 13,963 | Empty sequential string-table slots — excluded from gap count as irrecoverable |
| **Coverage gap total** | **12,664** | Partial + StringsOnly only |

`BlankSlot` was introduced this session to filter the 13,963 empty placeholder IDs from the 29,001-entry string files that were inflating the gap count.

### Identity Domains (as of commit 2a4c1bd7)

- `Race.EntryId`: Confirmed — canonical race string IDs
- `CareerLine.EntryId`: Confirmed — canonical career IDs 0–24, used by `abilityexport.bin` CareerLine field
- `CareerName.EntryId`: Confirmed — **NOT CareerId**; 5 display-context groups × 24 careers (IDs 12–131). Duplicates are expected and structurally explained.

### Remaining Open Work

- **Coverage gaps (12,664)**: inherent data gaps — Partial abilities have broken effect chains or missing CSV rows; StringsOnly have names but no client BIN evidence
- **SERVER_COMMAND field mapping**: field that holds the command code needs reconciliation — BIN analysis says FlagsRaw (8 distinct), Londos DB (`War_AbilityComponentBin.sql`) shows Values[0] (72+ distinct), Flags=0 always; likely a Londos remapping; 72+ command codes with per-code argument patterns now documented
- **Unknown op names**: ops 29, 30, 32, 40, 41, 43, 47, 51 — all absent from client-side `ComponentOP` enum (MYPLib) confirming they are server-side only. Full row counts and field patterns documented in `docs/data-matrix/overview/path-forward.md`. Op=43 confirmed MOVEMENT_AUTOATTACK (Londos desc="Can autoattack while moving."; live DB `MoveAndShoot` buff cmd; `BuffEffectInvoker.MoveAndShoot()` line 3145). Op=40: 2 rows, binary state toggle in standard-bearing chain. Op=41: 4 rows, Values=[14,187701–187706] same IDs as op=42 RECOVER_STANDARD. Op=47: 8 rows (4 pairs), Bite of the Skaven event context. Op=51: 38 rows, ability-reference + ordinal, includes "You are a coward!" debuff (ordinals 13/14) and battlefront/fortress ordinals 39–91. Op=29: 12 rows, Witch Hunter Accusations chain (IDs 66001–66300). Ops 30 (1 row, HEAD of Crack Shot) and 32 (15 rows, HEAD of Eye of Sheerian) still unresolved. No further naming possible without decompiled WarServer.dll (`WarServer.Game.Ability.Ext.Components` namespace).

See `docs/data-matrix/overview/path-forward.md` for full roadmap and source-search notes.

## External Data Locations

| Path | Contents |
|------|----------|
| `C:\Users\Admin\Downloads\myps` | Extracted WAR client files (gamedata CSV, strings TXT, BIN exports, zones, assetdb) |
| `C:\Users\Admin\Music\Warhammer` | Original MYP archives, PacketLogger, reference docs, WAR.exe |
| `C:\temp\world_extract` | Extracted world/zone asset tree (assetdb, zones) |
| `C:\temp\los` | Native-generated LOS output (zone 280 bin) |
| `C:\temp\los_baseline` | Baseline native LOS before parity fixes |
| `C:\temp\los_fix1` | Intermediate LOS fix checkpoint |
| `C:\temp\col_extract` | Extracted collision NIF files |
| `C:\temp\art_extract`, `art2_extract`, `art3_extract` | Extracted ART MYP content |
| `C:\temp\zone_fill` | Zone fill working area |
| `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit` | RE toolkit: decompiled client code, BIN export parsers, ability enums |

## Bot System

A fully integrated, player-like Bot System is now implemented to populate the world with autonomous entities. They participate in RvR and Scenarios, running with zero network overhead. Bots are automatically assigned to permanent race-specific faction guilds (e.g., Empire, Greenskins) and operate cohesively in tactical groups. See `BOT_SYSTEM.md` for detailed information on architecture, logic, and GM commands.

## System Guilds

A new automated guild system provides "Forces of Order" and "Forces of Destruction" as starter guilds for all new players. These guilds are automatically maintained at level 40. Players who leave are tracked to prevent re-entry via regular invites. See `docs/SYSTEM_GUILDS.md` for full details.

## Recently Resolved Issues

### T1 RvR Functionality Restored

`LowerTierCampaignManager.OpenActiveBattlefront()` now correctly calls `flag.OpenBattleFront()`. The FSM correctly starts, and flag captures now register as expected.

### Career Vendor Ability Purchase Fixed

The trainer packet handler has been updated to correctly map the 1-based index from the client to the list of unpurchased career abilities sorted by rank and name. Players now purchase the correct ability from vendors.
