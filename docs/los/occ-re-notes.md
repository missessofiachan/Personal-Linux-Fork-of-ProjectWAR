# OCC LOS Reverse Engineering Notes

These notes track what the shipped `los/*.bin` files contain and how they compare to the current native `LosBuilder` output.

## Format Summary

The shipped LOS files use the same custom `OCC` container that `WarZone` reads in `ZoneManager.cpp`.

- Header:
  - bytes `OCC`
  - version byte
  - header-size byte
- Chunk types:
  - `7` = region
  - `4` = terrain
  - `5` = collision
  - `8` = water

### Region Chunk

The region chunk contains:

- `RegionId`
- `ZoneCount`
- for each zone entry:
  - `ZoneId`
  - `OffsetX`
  - `OffsetY`
  - `ZoneNifCount`
  - `ZoneFixtureCount`

Current files appear to contain one zone entry per file.

### Terrain Chunk

The terrain chunk contains:

- `RegionId`
- `ZoneId`
- `Width`
- `Height`
- `HoleWidth`
- `HoleHeight`
- `ushort[Width * Height]` height samples
- `byte[HoleWidth * HoleHeight]` hole samples

Two terrain layouts exist in shipped data:

- `256x256` holemap present
- `0x0` holemap absent

### Collision and Water Chunks

Collision and water chunks share the same structure:

- `RegionId`
- `ZoneId`
- `VertexCount`
- `Vector3[VertexCount]`
- `FixtureCount`
- `TriangleCount`
- `IndexSize`
- `Triangle[TriangleCount]`

Each triangle stores:

- `i0`
- `i1`
- `i2`
- `uniqueID`

`uniqueID` layout matches runtime expectations:

- high 8 bits = `SurfaceType`
- low 24 bits = `FixtureId`

The triangle stream is grouped by `uniqueID`, and `WarZone` depends on that grouping when it builds fixture buckets at load time.

## Shipped Folder Survey

Survey of the current shipped runtime folder `bin/Release/los`:

- `249` files total
- `249` valid `OCC` files
- `129` files include a non-empty water chunk
- holemap histogram:
  - `124` files use `256x256`
  - `99` files use `0x0`

Collision density varies widely:

- smallest collision chunks include zones with `0` collision triangles
- largest sampled zone:
  - zone `201`
  - `2,749,332` collision triangles
  - `1,668,631` vertices
  - `11,540` unique fixtures

## Zone 280 Golden-Master Comparison

Shipped `280.bin` versus current native `LosBuilder` output for zone `280`:

- File size:
  - shipped: `2,284,132`
  - native: `6,225,992`
- Region offsets:
  - shipped: `753664,1277952`
  - native: `376832,638976`
- Terrain:
  - same `1024x1024` dimensions
  - same min/max height range: `4820..18996`
  - shipped has `0x0` holemap
  - native writes `256x256` holemap
  - terrain hashes do not match
- Collision:
  - shipped: `6,074` vertices, `7,115` triangles
  - native: `135,815` vertices, `152,088` triangles
  - both declare `77` fixtures and group triangles by `uniqueID`
- Water:
  - shipped has a water chunk with `6` vertices and `2` triangles
  - native currently emits no water chunk for zone `280`

### Zone 280 Bucket Pattern

Zone `280` shipped collision buckets are extremely uneven:

- `77` fixture buckets total
- `75` buckets use `33` triangles
- `1` bucket uses `1,788` triangles
- `1` bucket uses `2,852` triangles

The two large buckets are fixture ids `40` and `41`.

This is strong evidence that shipped LOS is not a naive dump of every raw fixture mesh triangle. It is either:

- built from alternate collision meshes, or
- passed through a simplification stage that the current native generator does not yet reproduce.

## Zone 201 Water Findings

Zone `201` is a useful water reference:

- `122` water triangles
- `24` water fixture buckets
- `4` water surface types

Observed water `uniqueID` examples:

- `218169344` = surface `13`, fixture `65536`
- `167837697` = surface `10`, fixture `65537`

This suggests water uses synthetic fixture ids starting around `65536`, with the surface type carried in the high byte exactly like collision fixtures.

## Current Native Generator Gaps

The comparison work so far points to four concrete parity gaps:

1. Region offsets are wrong.
   - For zone `280`, shipped offsets are exactly double the current native offsets.
2. Terrain serialization is not parity-correct.
   - Even when min/max heights match, the full height hash differs.
3. Holemap behavior is not parity-correct.
   - Many shipped files omit the holemap entirely.
4. Collision output is far too dense.
   - Native generation is closer to full mesh emission than to shipped LOS output.

## Practical Use

The shipped `los/*.bin` files are good enough to use as a golden master.

The recommended workflow is:

1. Inspect shipped bins with `LosBuilder inspect`.
2. Generate a native candidate for the same zone.
3. Compare shipped vs native with `LosBuilder compare`.
4. Fix native generation until chunk metadata, offsets, terrain layout, water layout, and collision density converge.
