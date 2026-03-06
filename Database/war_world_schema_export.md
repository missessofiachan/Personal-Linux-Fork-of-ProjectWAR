# `war_world` Schema Export

This repository now includes a schema-only export:

- `Database/war_world_schema.sql`

## What It Contains

- `CREATE DATABASE IF NOT EXISTS war_world`
- `USE war_world`
- Full table/view structure for `war_world`
- Triggers, routines, and events
- No table data (`--no-data` export)

## Import

```powershell
mysql -uroot -ppassword < Database\war_world_schema.sql
```

## Regenerate

```powershell
mysqldump -uroot -ppassword --databases war_world --no-data --routines --events --triggers --set-gtid-purged=OFF > Database\war_world_schema.sql
```
