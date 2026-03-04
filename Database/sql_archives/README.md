# SQL 7z archives

These archives are for storage/transfer footprint reduction.

## Files
- `master_war_world_parts.7z`
  - `Database/New_war_world/war_world_part1.sql`
  - `Database/New_war_world/war_world_part2.sql`
  - `Database/New_war_world/war_world_part3.sql`
- `restart_war_world_base_plus_update.7z`
  - `Database/war_world.sql`
  - `Database/updates/update_001.sql`
- `accounts_and_characters_base.7z`
  - `Database/war_accounts.sql`
  - `Database/war_characters.sql`
- `curated_selective_master_scripts.7z`
  - `Database/curated_selective_master/2026_03_03_apply_curated_plus_selective_master.sql`
  - `Database/curated_selective_master/README.md`

## Extract example
`"C:\Program Files\7-Zip\7z.exe" x Database\sql_archives\master_war_world_parts.7z -oDatabase\New_war_world`

