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

### Component Field Decode Status (as of commit c193be3e, 2026-03-28)

All 18,526 component records across all operation types have been fully decoded. **Unknown = 0, Structural = 0.**

Every field in `abilitycomponentexport.bin` is now at Confirmed or Inferred confidence:
- **ExtData Val1–Val4** (application target, operation type code, application profile, layout tag): universal cross-op decode
- **Values[0–7]**: per-operation semantics documented for all 40+ operation types, including all unnamed ops (29, 30, 32, 40, 41, 43, 47, 51)
- **Multipliers[0–7]**: scaling percentages decoded per operation
- **FlagsRaw**: CrowdControlTypes bitmasks, bit-field flags, sequential enums decoded per operation
- **Value08**: universal binary flag (= 1 across all operations)
- **Value15**: universal CC gate (CrowdControlTypes bits, mask 0x8FF) across all operations

Previously ambiguous fields now resolved:
- `DAMAGE Value[1]` — **Confirmed as `MaxCounter`** (counter cap / tick limit); from decompiled server `DAMAGE.cs`
- `DAMAGE FlagsRaw` — **Confirmed as `DamageFlag` enum** (NONE=0, UNMITIGATABLE=1); from same source

One field remains Inferred:
- `SERVER_COMMAND Value[2]` — 337 non-zero, 59 distinct values; polymorphic command argument (tri-modal: small enum / ID ref / sentinel)

All 558 requirement rows in `abilityrequirementexport.bin` are now decoded (Val1=AbilitySourceType, Val2=AbilityOperation, Val3=AbilityCondition, Val4=AbilityLogicOperator from decompiled `AbilityExport.cs`).

Remaining open work: coverage gaps (26,627 abilities below Mapped), SERVER_COMMAND Value[2] semantics, unknown op names for ops 29/30/32/40/41/43/47/51, CareerName identity domain.

See `docs/data-matrix/overview/path-forward.md` for the full roadmap.

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

## Recently Resolved Issues

### T1 RvR Functionality Restored

`LowerTierCampaignManager.OpenActiveBattlefront()` now correctly calls `flag.OpenBattleFront()`. The FSM correctly starts, and flag captures now register as expected.

### Career Vendor Ability Purchase Fixed

The trainer packet handler has been updated to correctly map the 1-based index from the client to the list of unpurchased career abilities sorted by rank and name. Players now purchase the correct ability from vendors.
