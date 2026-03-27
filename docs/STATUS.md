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

Source files it reads are all under `C:\Users\Admin\Pictures\WAR_extracted`:
- `data/gamedata/abilities.csv`, `effects.csv`, `pregame_chars.xml`
- `data/strings/english/abilitynames.txt`, `abilitydesc.txt`, `abilityeffect.txt`, `careernames_m.txt`, `careerlines_m.txt`, `racenames_m.txt`
- `data/bin/abilityexport.bin`, `abilitycomponentexport.bin`, `abilityrequirementexport.bin`

Reports are generated at runtime to a local output directory. They are not committed to the repo.

Tool usage: `docs/client-data-matrix-usage.md`.

## External Data Locations

| Path | Contents |
|------|----------|
| `C:\Users\Admin\Pictures\WAR_extracted` | Extracted WAR client files (gamedata CSV, strings TXT, BIN exports, zones, assetdb) |
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

A fully integrated, player-like Bot System is now implemented to populate the world with autonomous entities. They participate in RvR and Scenarios, running with zero network overhead. See `BOT_SYSTEM.md` for detailed information on architecture, logic, and GM commands.

## Open Bugs

### T1 RvR non-functional

`LowerTierCampaignManager.OpenActiveBattlefront()` calls `flag.SetObjectiveSafe()` instead of `flag.OpenBattleFront()`. The FSM never starts, so flag captures do not register. Fix: replace `SetObjectiveSafe()` with `OpenBattleFront()` at the call site (~line 258).

### Career vendor ability purchase — wrong ability purchased

The client sends the selection by index. The server list order may not match the client display order. The trainer packet handler needs investigation for list-order mismatch.
