# ProjectWAR Emulator Beginner Guide

This guide is written for complete beginners.

## What this project does

ProjectWAR is a local server emulator for Warhammer Online. It lets you run the game server stack on your own computer so you can test and play in a private local environment.

## How the emulator is organized

When you start the emulator, these services work together:

- `AccountCacher`: account/login data and RPC hub
- `LauncherServer`: patch/login handoff service
- `LobbyServer`: client lobby connection
- `WorldServer`: game world and gameplay logic

Your local launcher/client connects to these services on `127.0.0.1` (your own machine).

## One-time checklist

Install these first:

- Windows
- Visual Studio 2022 with `.NET desktop development`
- .NET Framework 4.8 Developer Pack
- MySQL or MariaDB (local)
- Warhammer Online client files

## Setup steps (follow in order)

### 1. Download zone data

Zone files are too large for git and must be downloaded separately.

1. Download `zones.zip` from:
   - https://github.com/Shmerrick/ProjectWAR/releases/tag/zones-data-v1
2. Extract to `deps/zones/`

Checkpoint: you should see folders like `deps/zones/zone001/`.

### 2. Create and import databases

Create three databases:

- `war_accounts`
- `war_characters`
- `war_world`

Import these SQL files:

- `Database/war_accounts.sql`
- `Database/war_characters.sql`
- `Database/war_world.sql`

Example commands:

```powershell
mysql -u root -p -e "CREATE DATABASE war_accounts; CREATE DATABASE war_characters; CREATE DATABASE war_world;"
mysql -u root -p war_accounts < Database/war_accounts.sql
mysql -u root -p war_characters < Database/war_characters.sql
mysql -u root -p war_world < Database/war_world.sql
```

Checkpoint: all three databases exist and contain tables.

### 3. Build the solution

1. Open `WarEmulator.sln` in Visual Studio.
2. Set build configuration to `Release` and platform to `x64`.
3. Build the solution.

Checkpoint: build output is in `bin/Release/`.

### 4. Verify local config files

Check these files in `bin/Release/Configs/`:

- `Account.xml`
- `Launcher.xml`
- `Lobby.xml`
- `World.xml`
- `mythloginserviceconfig.xml`

Default local values:

- host `127.0.0.1`
- DB port `3306`
- DB user `root`
- DB password `password`

If your DB password is different, update:

- `bin/Release/Configs/Account.xml`
- `bin/Release/Configs/World.xml`

### 5. Start server services

```powershell
.\start_servers.ps1
```

Checkpoint: these processes should be running:

- `AccountCacher`
- `LauncherServer`
- `LobbyServer`
- `WorldServer`

### 6. Start the game launcher

1. Open `launch_client.ps1`.
2. Replace the launcher path with your local WAR launcher path.
3. Run:

```powershell
.\launch_client.ps1
```

## Quick health checks

Check running emulator services:

```powershell
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer' } | Select-Object Name, Id
```

Check recent application errors:

```powershell
.\check_crash.ps1
```

## Stop all services

```powershell
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer' } | Stop-Process -Force
```

## Troubleshooting

- Build fails or package errors:
  - restore NuGet packages in Visual Studio, then rebuild.
- Database connection failures:
  - verify credentials in `Account.xml` and `World.xml`.
  - verify MySQL/MariaDB is running on `127.0.0.1:3306`.
- Client cannot connect:
  - verify all four services are running.
  - verify config files still point to localhost values.
- Missing terrain/zone data:
  - verify `deps/zones/` extraction.
  - rebuild so assets are copied into `bin/Release/zones/`.

## Legacy reference

The previous README is archived at:

- `docs/archive/README-legacy-2026-02-24.md`
