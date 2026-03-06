# Mythic CSV Snapshot

This folder stores the client CSV snapshot used by the ability parity workstream.

- Source path: `C:\Users\Admin\Pictures\WAR\hash_workspace\extracted\data\data\gamedata`
- Sync script: `Database/ability_parity/Sync-MythicAbilityCsvBundle.ps1`
- Imported DB tables: `mythic_csv_dataset_meta`, `mythic_csv_raw_rows`, `mythic_csv_abilities`, `mythic_csv_effects`

The importer copies selected CSVs into `Database/ability_parity/csv/gamedata/` and records per-file hash and row counts in `mythic_csv_dataset_meta` for verification.
