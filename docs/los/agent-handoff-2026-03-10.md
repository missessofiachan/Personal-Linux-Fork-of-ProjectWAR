# LOS Agent Handoff - 2026-03-10

This is the current checkpoint for future AI agents continuing LOS reverse engineering and native generation work.

## Branch

- `ability-parity-mythic-v1`

## What Was Added

- Native LOS generator project:
  - `LosBuilder/`
- Solution/build integration:
  - `ProjectWAR.sln`
  - `WorldServer/WorldServer.csproj`
- LOS `OCC` inspection and compare tooling:
  - `LosBuilder/Generation/OccReader.cs`
  - `LosBuilder/Generation/OccInspector.cs`
  - `LosBuilder/Generation/OccModels.cs`
- Docs:
  - `docs/los/README.md`
  - `docs/los/occ-re-notes.md`

## What Is Confirmed

- Shipped runtime LOS files are valid and consistent:
  - `249` valid `OCC` files in `bin/Release/los`
- Runtime format is stable:
  - `7 = region`
  - `4 = terrain`
  - `5 = collision`
  - `8 = water`
- `uniqueID` is encoded as:
  - `surfaceType << 24 | fixtureId`
- Triangle streams are grouped by `uniqueID`, and `WarZone` depends on that grouping.
- Shipped LOS is currently better than native LOS output.

## Most Important Comparison

Zone `280` is the clean reference zone because native generation succeeded there.

Shipped `bin/Release/los/280.bin`:

- `2,284,132` bytes
- offsets: `753664,1277952`
- terrain: `1024x1024`, no holemap chunk data (`0x0`)
- collision: `6074` vertices, `7115` triangles
- water: present, `6` vertices, `2` triangles

Native generated zone `280`:

- `6,225,992` bytes
- offsets: `376832,638976`
- terrain: `1024x1024`, `256x256` holemap
- collision: `135815` vertices, `152088` triangles
- water: absent

Conclusion:

- native offsets are wrong
- native terrain serialization is not parity-correct
- native holemap behavior is not parity-correct
- native collision output is far too dense
- native water generation is incomplete

## Global Shipped LOS Survey

From `bin/Release/los`:

- `129` files include water chunks
- holemap distribution:
  - `124` files use `256x256`
  - `99` files use `0x0`

This means the shipped files use at least two terrain/holemap variants. Do not assume a holemap is always present.

## What To Read First

1. `docs/los/README.md`
2. `docs/los/occ-re-notes.md`
3. `WarZone/WarZone/ZoneManager.cpp`
4. `LosBuilder/Generation/LosGenerator.cs`
5. `LosBuilder/Generation/OccWriter.cs`
6. `LosBuilder/Generation/OccReader.cs`
7. `LosBuilder/Generation/OccInspector.cs`

## Best Next Goals

1. Fix region offset parity.
   - Zone `280` strongly suggests a scale mismatch.
2. Fix terrain parity.
   - Match shipped terrain hash, not just width/min/max.
3. Fix holemap behavior.
   - Native generator should be able to emit either `0x0` or `256x256` depending on zone behavior.
4. Fix water parity.
   - Native output for zone `280` is missing water even though shipped output has it.
5. Reduce collision density.
   - Shipped output is dramatically smaller than native output and appears simplified or collision-mesh-based.

## Recommended Workflow

1. Build `LosBuilder`.
2. Pick one shipped reference zone.
3. Run `inspect` on shipped file.
4. Generate native file for the same zone.
5. Run `compare`.
6. Fix one mismatch class at a time.

## Commands

Build:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe LosBuilder\LosBuilder.csproj /p:Configuration=Release /p:Platform=x64
```

Inspect:

```powershell
bin\Release\LosBuilder.exe inspect --input-bin bin\Release\los\280.bin
```

Compare:

```powershell
bin\Release\LosBuilder.exe compare --left-bin bin\Release\los\280.bin --right-bin C:\temp\los\280.bin
```

Generate one zone:

```powershell
bin\Release\LosBuilder.exe generate --input-root C:\Users\Admin\Pictures\WAR_extracted --output-root C:\temp\los --zone 280
```

## Notes On Source Data

- `C:\Users\Admin\Pictures\WAR_extracted\assetdb` has the mesh library and `figleaf.db`.
- The extracted `zones` tree is only partially complete for the raw zone files the current generator expects.
- This is why zone `280` works but full all-zone native generation does not.

## Do Not Do

- Do not treat the current native output as authoritative.
- Do not rewrite the shipped LOS files.
- Do not assume the runtime `zones` PNG folder can replace raw LOS source data.

The shipped `los/*.bin` files are the reference target until native parity is proven.
