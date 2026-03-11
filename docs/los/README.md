# LOS Work README

This folder tracks the server-side line-of-sight (`LOS`) and occlusion reverse-engineering work for `ProjectWAR`.

## Current State

- Runtime LOS is implemented by `WarZone64.dll`.
- Runtime data is loaded from `bin/Release/los/*.bin`.
- Those `.bin` files use the custom `OCC` container format read by `WarZone/WarZone/ZoneManager.cpp`.
- `LosBuilder` can now:
  - generate native LOS from extracted WAR assets
  - inspect shipped or generated `OCC` files
  - compare two `OCC` files directly

## Key Files

- Runtime loader:
  - `WarZone/WarZone/ZoneManager.cpp`
  - `WorldServer/World/Map/Occlusion.cs`
- Native generator:
  - `LosBuilder/Generation/LosGenerator.cs`
  - `LosBuilder/Generation/OccWriter.cs`
- Reverse-engineering helpers:
  - `LosBuilder/Generation/OccReader.cs`
  - `LosBuilder/Generation/OccInspector.cs`
  - `LosBuilder/Generation/OccModels.cs`
- Notes:
  - `docs/los/occ-re-notes.md`
  - `docs/los/agent-handoff-2026-03-10.md`

## Useful Commands

Build `LosBuilder`:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe LosBuilder\LosBuilder.csproj /p:Configuration=Release /p:Platform=x64
```

Inspect a shipped LOS file:

```powershell
bin\Release\LosBuilder.exe inspect --input-bin bin\Release\los\280.bin
```

Compare shipped vs native output:

```powershell
bin\Release\LosBuilder.exe compare --left-bin bin\Release\los\280.bin --right-bin C:\temp\los\280.bin
```

Generate one zone natively:

```powershell
bin\Release\LosBuilder.exe generate --input-root C:\Users\Admin\Pictures\WAR_extracted --output-root C:\temp\los --zone 280
```

## What Matters

The shipped `los/*.bin` files are currently the golden master.

The native generator is structurally correct, but not yet parity-correct. The main remaining gaps are:

- region offset scaling
- terrain and holemap serialization parity
- water chunk generation parity
- collision simplification parity

Future LOS work should use the shipped files as the oracle and move the native generator toward those outputs incrementally.
