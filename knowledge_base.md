# ProjectWAR Linux Development Knowledge Base

This document serves as a comprehensive knowledge base detailing all the fixes, modifications, configuration adjustments, and scripting tools implemented to successfully compile, run, and connect a client to the ProjectWAR server codebase on Linux (Fedora/Bazzite) via a distrobox container.

---

## 1. Database Configuration & Imports

### Database Schema Alignment
*   **Problem:** The database dump script `war_world.sql` inside the `Database/` directory had hardcoded instructions creating and using a target database schema named `war_world_curated`.
*   **Resolution:** Modified all instances of `war_world_curated` to point directly to `war_world` to match the default configuration name in the server XMLs.
*   **Result:** Allowed correct population of all 18 RvR Tier 1 and Tier 4 progression records.

---

## 2. C++ WarZone Shared Library Port

### Compiling on Linux
*   **Header Adjustments:** Modified `LinuxPort.h` inside the `WarZone` project to include missing POSIX standard library definitions (such as string manipulation and offset utilities).
*   **Compilation:** The native shared library `libWarZone.so` is compiled via `CMake` and `make` using `libglm-dev` headers within the container.
*   **Deployment:** The built `libWarZone.so` is copied directly to the server executable runtime directory: `bin/Debug/`.

### Mono DLL Mapping
*   **Config modification:** Added the following dllmap redirection inside [WorldServer.exe.config](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/WorldServer.exe.config) to map P/Invoke calls looking for `WarZone.dll` to find the compiled C++ shared object:
    ```xml
    <configuration>
      <dllmap dll="WarZone" target="libWarZone.so" />
    </configuration>
    ```

---

## 3. Server-Side Safety & Stability Patches

Due to database schema discrepancies and pre-loading configurations, several `NullReferenceException` crashes were resolved in the C# server code:

### A. Campaign Objectives Safeties
*   **File:** [Campaign.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Battlefronts/Apocalypse/Campaign.cs)
*   **PlaceObjectives():** Added null checks when placing region objectives. This prevents crashes if a preloaded Tier 1 region is configured in the database without standard battlefield objectives.
*   **UpdateBOs():** Added safety checks to prevent iteration over `Objectives` when the list evaluates to `null`.

### B. Keep Loader Safeties
*   **File:** [BattleFrontKeep.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Battlefronts/Keeps/BattleFrontKeep.cs)
*   **Guild Flag Mapping:** Modified the search on `GetBattleFrontObjectives(Region.RegionId)` to safely check for null lists before invoking `.SingleOrDefault()`.
*   **Spawn Point Dictionary:** Swapped direct Linq evaluation on `_PlayerKeepSpawnPoints` (which caused null-reference exceptions on empty entries) to use a safe `TryGetValue` lookup instead.

### C. Campaign Manager Locking Safeties
*   **Files:** 
    *   [UpperTierCampaignManager.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Battlefronts/Apocalypse/UpperTierCampaignManager.cs)
    *   [LowerTierCampaignManager.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Battlefronts/Apocalypse/LowerTierCampaignManager.cs)
*   **LockBattleFrontsAllRegions():** Wrapped the loops targeting battlefield locking objectives in null-safety checks to prevent crashing when pre-loading regions that lack objective entries.

### D. Keep Communications Door Safeties
*   **File:** [KeepCommunications.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Battlefronts/Keeps/KeepCommunications.cs)
*   **SendKeepStatus():** Added null guards for `innerDoor` and its internal `GameObject` reference before evaluating door health percentages (`PctHealth`).

### E. Launcher XML Configuration Fix
*   **File:** [Launcher.xml](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/bin/Debug/Configs/Launcher.xml)
*   **Malformed tags:** Corrected a missing closing XML tag for the `<Message>` property (`<Message>Invalid launcher version.`) which crashed the launcher configuration reader on start.

---

## 4. Automation & Orchestration Scripts

We created four executable scripts in the repository root to handle system compilation and tmux process orchestration:

*   **[compile.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/compile.sh):**
    Automatically compiles both the C++ physics engine and the C# server assemblies inside the distrobox container, moving dependencies into the target runtime directories.
*   **[start-db.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/start-db.sh):**
    Starts the host machine's MariaDB database server.
*   **[start-servers.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/start-servers.sh):**
    Launches the 4 server processes (`AccountCacher.exe`, `LauncherServer.exe`, `LobbyServer.exe`, and `WorldServer.exe`) in separate panes inside a container-based `tmux` session named `projectwar`.
*   **[start-all.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/start-all.sh):**
    A master script to start the database server, wait for boot, and start the game server cluster.

---

## 5. Game Client Connections (Return of Reckoning)

Connecting the Return of Reckoning client directly to your local server cluster requires three key setups:

### A. Windowed Mode Force Configuration
The DirectX 9 renderer can crash when launching the 32-bit executable directly on Linux due to exclusive fullscreen hooks failing under Proton/DXVK. 
*   Created default `settings.xml` configurations to force windowed mode in both:
    *   `/var/mnt/nvme0n1p4/return of reckoning/drive_c/Program Files (x86)/Return of Reckoning/user/UserSettings.xml`
    *   `~/Electronic Arts/Warhammer Online/user/settings.xml`

### B. Host Network Redirection (NAT Route)
The Return of Reckoning client executable has its login endpoints hardcoded to its public servers (`147.135.130.126`). To redirect client authentication traffic to your local server without modifying the game files, apply this routing rule on the host:
```bash
sudo iptables -t nat -A OUTPUT -d 147.135.130.126 -j DNAT --to-destination 127.0.0.1
```
*(To remove this redirect when you want to connect back to their public servers, run: `sudo iptables -t nat -D OUTPUT -d 147.135.130.126 -j DNAT --to-destination 127.0.0.1`)*.

### C. Client Launch Arguments & Account Session Matching
*   **Lutris Config:** Set up your custom Lutris shortcut with the following properties:
    *   **Executable:** `/var/mnt/nvme0n1p4/return of reckoning/drive_c/Program Files (x86)/Return of Reckoning/WAR-64.exe`
    *   **Wine Prefix:** `/var/mnt/nvme0n1p4/return of reckoning`
    *   **Arguments:**
        ```text
        --clp 127.0.0.1:8048 --acctname=c29maWFjaGFu --sesstoken=ODNmMTEzZWM4NQ==
        ```
*   **Database Record Setup:** Created the developer account (`sofiachan`) and set the matching base64 session token in the database:
    *   **Account:** `sofiachan` (Base64: `c29maWFjaGFu`)
    *   **Token:** `83f113ec85` (Base64: `ODNmMTEzZWM4NQ==`)
    *   **GM Level:** `40`
