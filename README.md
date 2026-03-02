# ProjectWAR Emulator Beginner Guide

This guide is written for complete beginners.

## What this project does

ProjectWAR is a local server emulator for Warhammer Online. It lets you run the game server stack on your own computer so you can test and play in a private local environment.

The current focus of the project is the **1.4.8 Restoration Plan**, which aims to restore the emulator's database and logic to the state of the final official patch (1.4.8) using authentic "Sources of Truth".

## How the emulator is organized

When you start the emulator, these services work together, managed by the **ServerLauncher**:

- `ServerLauncher`: The central GUI application to start, monitor, and stop all server services.
- `AccountCacher`: account/login data and RPC hub.
- `LauncherServer`: patch/login handoff service.
- `LobbyServer`: client lobby connection.
- `WorldServer`: game world and gameplay logic.

Your local launcher/client connects to these services on `127.0.0.1` (your own machine).

## One-time checklist

Install these first:

- Windows
- Visual Studio 2022 with `.NET desktop development`
- .NET Framework 4.8 Developer Pack
- MySQL
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

**IMPORTANT**: ALWAYS use `ServerLauncher.exe` to start the server stack. Do NOT start individual executables separately, as they will not initialize correctly.

1. Navigate to `bin/Release/`.
2. Run `ServerLauncher.exe`.
3. Click "Start All" (or individual start buttons in order: Account, Launcher, Lobby, World).

### 6. Start the game launcher

Start your local Warhammer Online client.

## Quick health checks

Check running emulator services:

```powershell
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer|ServerLauncher' } | Select-Object Name, Id
```

## Stop all services

Use the "Stop All" button in `ServerLauncher.exe`, or force stop via PowerShell:

```powershell
Get-Process | Where-Object { $_.Name -match 'AccountCacher|LauncherServer|LobbyServer|WorldServer|ServerLauncher' } | Stop-Process -Force
```

## Troubleshooting

- Build fails or package errors:
  - restore NuGet packages in Visual Studio, then rebuild.
- Database connection failures:
  - verify credentials in `Account.xml` and `World.xml`.
  - verify MySQL/MariaDB is running on `127.0.0.1:3306`.
- Client cannot connect:
  - verify all services are running via `ServerLauncher`.
  - verify config files still point to localhost values.
- Missing terrain/zone data:
  - verify `deps/zones/` extraction.
  - rebuild so assets are copied into `bin/Release/zones/`.

## Developer Documentation

For contributors and AI agents, please refer to the following architectural and restoration documents:

- **[1.4.8 Restoration Plan](1_4_8_RESTORATION_PLAN.md)**: Details the project's core mission and data restoration workflow.
- **[Systemic Divergences](SYSTEMIC_DIVERGENCES.md)**: Maps differences between Retail and Emulator logic.
- **[AI Documentation](AI_Documentation/AI_README.md)**: Central repository for AI rules, instructions, and change logs.

## Development Resources

When analyzing network protocols, game assets, structures, or looking for reverse engineering findings, all contributors and AI agents should reference the **WAR-RE-Toolkit** repository:
- **Local Path**: `C:\Users\Admin\source\repos\Shmerrick\WAR-RE-Toolkit`
- **Remote**: Private GitHub repo `Shmerrick/WAR-RE-Toolkit`

This toolkit contains essential companion tools like `WarClientTool`, `AssetHashHunter`, `Diffuser`, and various database scripts required for emulator improvement.

### Database Modification Rules

**CRITICAL RULE FOR ALL CONTRIBUTORS AND AI AGENTS:**

1. **NEVER modify** the base `.sql` files located in the `Database/` folder (`war_accounts.sql`, `war_characters.sql`, `war_world.sql`). These are meant for the initial setup by end-users.
2. If a source code change requires a database schema or data modification, you **MUST create a new update script** (e.g., `update_001.sql`).
3. These update scripts should be provided alongside the code changes, and end-users must be prompted to apply them to their database prior to loading the emulator for the server to run correctly.

## RvR Terminology

### Global Concepts
- **Battlefield Objective (BO)**: A location on the RvR section of a map (zone) that players must control. Typically represented by a flag.
- **Keep**: A large-scale BO with deeper capture and hold mechanics (guards, doors, lords).
- **Faction**: The two opposing sides: **Order** (Dwarf, Empire, High Elf) and **Destruction/Chaos** (Greenskin, Chaos, Dark Elf).
- **Race**: Specific ethnic groups within factions (e.g., Dwarf vs Greenskin).
- **Pairing**: A specific conflict between an Order race and its corresponding Destruction rival (e.g., Dwarf vs Greenskin).
- **Tier**: Level-bracketed gameplay areas (1-4) consisting of one or more zones.
- **Zone**: An individual map with a unique ID.
- **Battlefront**: The active RvR area of a pairing and its associated tier. A battlefront can span one or multiple zones.

### Flag States
- **Unclaimed**: The default state; no faction holds the flag.
- **Contested**: A faction has interacted with the flag, triggering a countdown timer. The opposing faction can attempt to reclaim it during this period.
- **Captured**: The countdown timer has reached zero. The claiming faction now owns the flag. It enters a **lockout state** where the opposing faction cannot interact with it. This triggers a lockout countdown timer.
- **Secured**: The lockout timer has reached zero. The faction still controls the flag, but it is now open for conflict and can be assaulted by the opposing faction.

### Domination
- **Domination Status**: Occurs when a single faction controls **all** Battlefield Objectives within a battlefront, and all those objectives are in the **Secured** state.
- **Domination Victory**: Domination supersedes the Victory Point (VP) requirement for controlling and locking a battlefront.
- **Rewards & Progression**: Achieving domination awards RvR lock rewards to the winning faction. The battlefront locks for 30 minutes before shifting to a new battlefront within the same tier.
