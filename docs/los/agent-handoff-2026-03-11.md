# LOS Agent Handoff - 2026-03-11

This is the current checkpoint for future AI agents continuing LOS native generation work.

## Branch

- `ability-parity-mythic-v1`

## What Was Fixed in This Session

All four structural parity gaps from the previous handoff are now resolved.

### Fix 1: Region Offsets (`FigleafMetadataReader.cs`)

Figleaf.db stores zone cell positions in 8192-unit cells.
- Before: `OffsetX = zoneXOff << 12`
- After: `OffsetX = zoneXOff << 13` (same for Y)
- Verified: `92 << 13 = 753664` matches zone `280` shipped offset.

### Fix 2: Terrain X-Axis Orientation (`TerrainRasterReader.cs`)

The terrain height reader was applying an X-axis flip that does not exist in the shipped data.
- Before: `int sourceX = width - x - 1;`
- After: `int value = scaleFactor * terrain.GetValue(x, y) + offsetFactor * offset.GetValue(x, y);`
- Verified: terrain height hash now matches `0x3E95E510C190B6D2`.

### Fix 3: Holemap Behavior (`TerrainRasterReader.cs`)

When `holemap.pcx` is absent, shipped files use `0×0` (no holemap). The generator was
emitting a `256×256` all-ones fallback.
- Before: fallback `holeWidth = width/4, holeHeight = height/4` with all-ones fill
- After: `holeWidth = 0, holeHeight = 0` when holemap.pcx is absent
- Verified: hole dimensions and hash now match shipped.

### Fix 4: Collision Density (`TriangleWalker.cs` + `LosGenerator.cs`)

The shipped LOS tool uses only the invisible (collision-flagged) geometry nodes inside WAR
fixture NIFs. Visual geometry nodes are visible (flag bit 0 = 0); collision proxies are
invisible (flag bit 0 = 1). The generator was loading all geometry.

- Added `GetCollisionTrianglesFromNode()` to `TriangleWalker.cs` — collects only invisible
  `NiTriBasedGeometry` nodes.
- Updated `LoadMesh()` in `LosGenerator.cs` to use collision-only geometry; falls back to all
  geometry only when no invisible nodes exist.
- Verified: tree proxies produce exactly `33` triangles per fixture (11-sided bounding cylinder
  embedded in the NIF as an invisible node). Tower proxies produce `1788` and `2852` triangles.
- Zone `280` triangle hash now matches shipped: `0xBAAEA5D12555DEB1`.

### Key Discovery: Collision NIFs Do Not Exist as Separate Files

Figleaf.db lists collision source names like `he_el_tree01_var01_collision.nif` and
`he_tower01_collision.nif`. These files do **not** exist in any WAR MYP archive (ART, ART2,
ART3 all searched). The collision geometry is embedded inside the visual NIF as invisible
sub-nodes. The `AssetIndex` falls back to the visual NIF which is correct — the invisible
nodes inside that NIF are the collision proxies.

## Zone 280 Current Compare Summary

```
Region:   all fields match ✓
Terrain:  size/holemap/minmax/heightHash/holeHash all match ✓
Collision:
  vertices:      6074 (match) ✓
  triangles:     7115 (match) ✓
  triangleHash:  0xBAAEA5D12555DEB1 (match) ✓
  vertexHash:    mismatch (~16 units max Y positional error)
  bucket counts: all 77 buckets match ✓
Water:    absent (zone 280 has no water.xml in extracted source data)
Length:   2284132 (shipped) vs 2283996 (native) — 136-byte gap = water chunk
```

## What Is Now Confirmed

- Shipped LOS collision geometry = invisible NiTriShape nodes inside the fixture NIF.
- 75 tree fixtures in zone `280` each produce exactly `33` triangles from their invisible nodes.
- 2 tower fixtures produce `1788` and `2852` triangles respectively from their invisible nodes.
- These counts exactly match the shipped file.

## Remaining Open Items

1. **Vertex position precision** — vertex hash mismatches due to NIF world-matrix accumulation
   in `GetWorldMatrixFromNode`. Max Y error ~16 units. Topology is correct (triangle hash
   matches). Minor positional bias, acceptable for LOS accuracy.

2. **Water generation** — zone `280` has no `water.xml`, so no water chunk is generated.
   Zone `201` is the reference water zone but lacks terrain PCX in extracted data.

3. **Multi-zone source data** — only zone `280` has all required files (terrain.pcx, offset.pcx,
   fixtures.csv, nifs.csv) in the current extracted data set at
   `C:\Users\Admin\Pictures\WAR_extracted\zones\`. More extraction work required to test
   other zones.

## Files Changed

- `LosBuilder/Generation/FigleafMetadataReader.cs` — offset shift fix (`<< 13`)
- `LosBuilder/Generation/TerrainRasterReader.cs` — holemap and X-flip fixes
- `LosBuilder/Vendor/Niflib/TriangleWalker.cs` — added `GetCollisionTrianglesFromNode()`
- `LosBuilder/Generation/LosGenerator.cs` — updated `LoadMesh()` to use collision nodes
- `docs/los/occ-re-notes.md` — updated with resolved gaps and new findings
- `docs/los/agent-handoff-2026-03-11.md` — this file

## Best Next Goals

1. **Resolve vertex position precision** — investigate `GetWorldMatrixFromNode` transform
   accumulation order (currently: R × S × T bottom-up; NIF convention may differ). A correct
   transform would close the vertex hash gap.

2. **Acquire more complete zone extractions** — expand the extracted zones beyond zone `280`
   to enable multi-zone testing. Consider extracting raw zones from WORLD.myp or supplementing
   from another MYP extraction pass.

3. **Water parity for zone `201`** — once zone `201` source data is available, verify the
   water chunk generation against shipped. Zone `201` has `122` water triangles across `24`
   buckets in shipped.

## Commands

Build:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe LosBuilder\LosBuilder.csproj /p:Configuration=Release /p:Platform=x64
```

Generate zone 280:

```powershell
bin\Release\LosBuilder.exe generate --input-root C:\Users\Admin\Pictures\WAR_extracted --output-root C:\temp\los --zone 280
```

Compare shipped vs native zone 280:

```powershell
bin\Release\LosBuilder.exe compare --left-bin bin\Release\los\280.bin --right-bin C:\temp\los\280.bin
```

Inspect shipped zone 280:

```powershell
bin\Release\LosBuilder.exe inspect --input-bin bin\Release\los\280.bin
```

## Source Data Notes

- `C:\Users\Admin\Pictures\WAR_extracted\assetdb` — NIF mesh library and figleaf.db.
- `C:\Users\Admin\Pictures\WAR_extracted\zones` — zone source folders; only zone `280` complete.
- `C:\Users\Admin\Music\Warhammer\Warhammer Online - Age of Reckoning` — original MYP files.
- ART2.myp contains fixture NIFs under `assetdb/fixtures/` without the `fi.0.0.` prefix.
  These are identical in content to the `fi.0.0.`-prefixed NIFs in WAR_extracted.

## Do Not Do

- Do not treat the current native output as authoritative.
- Do not rewrite the shipped LOS files.
- Do not assume the `_collision.nif` suffix files exist separately — they do not.
  Collision geometry lives inside the visual NIF as invisible nodes.
