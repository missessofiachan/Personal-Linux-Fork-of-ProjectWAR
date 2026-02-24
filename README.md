# ProjectWAR Emulator (Beginner Guide)

## What this project is

ProjectWAR is a **server emulator** for Warhammer Online.

In simple terms: the original game servers are gone, and this project provides replacement server software so you can run a local test world on your own computer.

## What you can do with it

- Start local game server services
- Connect a WAR client to your local server
- Test gameplay/database/script changes

## Before you start (one-time requirements)

You need:

- A Windows PC
- Visual Studio 2022 (with `.NET desktop development`)
- .NET Framework 4.8 Developer Pack
- MySQL or MariaDB running locally (default config uses `127.0.0.1:3306`)
- A local WAR client/launcher install

## Important files in this repo

- `WarEmulator.sln`: Visual Studio solution
- `Database/war_accounts.sql`: account database
- `Database/war_characters.sql`: character database
- `Database/war_world.sql`: world database
- `start_servers.ps1`: starts all required server executables
- `launch_client.ps1`: launches your local WAR launcher

## Step-by-step setup

### 1. Download zone data

Zone data is not included in git because it is large.

1. Download `zones.zip` from:  
   [https://github.com/Shmerrick/ProjectWAR/releases/tag/zones-data-v1](https://github.com/Shmerrick/ProjectWAR/releases/tag/zones-data-v1)
2. Extract it to `deps/zones/`

After extraction, you should have folders like:

- `deps/zones/zone001/`
- `deps/zones/zone002/`

### 2. Create and import databases

The default config expects these databases:

- `war_accounts`
- `war_characters`
- `war_world`

If you use the MySQL command line, this is the basic flow:

```powershell
mysql -u root -p -e "CREATE DATABASE war_accounts; CREATE DATABASE war_characters; CREATE DATABASE war_world;"
mysql -u root -p war_accounts < Database/war_accounts.sql
mysql -u root -p war_characters < Database/war_characters.sql
mysql -u root -p war_world < Database/war_world.sql
```

### 3. Build the solution

1. Open `WarEmulator.sln` in Visual Studio.
2. Set build configuration to `Release` and platform to `x64`.
3. Build the solution.

Build output is written to `bin/Release/`.

### 4. Verify config files

Check these files in `bin/Release/Configs/`:

- `Account.xml`
- `World.xml`
- `Launcher.xml`
- `Lobby.xml`
- `mythloginserviceconfig.xml`

Defaults use:

- DB server: `127.0.0.1`
- DB port: `3306`
- DB user: `root`
- DB password: `password`

If your local database password is different, update the XML files before running.

### 5. Start emulator servers

Run:

```powershell
.\start_servers.ps1
```

This launches:

- `AccountCacher`
- `LauncherServer`
- `LobbyServer`
- `WorldServer`

### 6. Start your game launcher

The script `launch_client.ps1` currently points to a local path.

1. Open `launch_client.ps1`
2. Replace the launcher path with your own WAR launcher location
3. Run:

```powershell
.\launch_client.ps1
```

## Stopping servers

If needed, stop them with:

```powershell
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer' } | Stop-Process -Force
```

## Basic troubleshooting

- Build fails with missing packages:
  - In Visual Studio, restore NuGet packages and rebuild.
- Server cannot connect to database:
  - Recheck DB credentials in `bin/Release/Configs/Account.xml` and `bin/Release/Configs/World.xml`.
- Client cannot connect:
  - Confirm server processes are running.
  - Confirm launcher config points to `127.0.0.1` and the expected ports.
- Missing world/zone assets:
  - Recheck `deps/zones/` extraction.

## Historical documentation

The previous README has been archived at:

- `docs/archive/README-legacy-2026-02-24.md`
