# ProjectWAR Optimization Summary

This document details the performance optimizations implemented to resolve CPU hotspots and database bottlenecks in the ProjectWAR server emulator.

---

## 1. Database Reflection Caching

### Problem ("The Why")
In the original database handler implementation (`ObjectDatabase.cs` and `MysqlObjectDatabase.cs`), whenever records were loaded or saved, the code performed raw .NET reflection calls (such as `GetMembers()` and `GetCustomAttributes()`) dynamically inside hot loops for every row and field.
Because reflection is computationally expensive, this caused severe CPU bottlenecks under heavy database traffic (e.g., loading game maps, saving characters, or batch transactions).

### Solution ("The What")
We replaced all dynamic reflection searches on these hot paths with lookups against `GetBindingInfo()`, which already caches type metadata upon initialization. 

### Files Modified
* **[ObjectDatabase.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/FrameWork/Database/ObjectDatabase.cs)**:
  * Refactored `FillObjectWithRow`, `FillRowWithObject`, `SaveObjectRelations`, and `DeleteObjectRelations` to iterate over cached `BindingInfo[]` instead of calling `GetMembers` and `GetCustomAttributes`.
* **[MysqlObjectDatabase.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/FrameWork/Database/Handler/MysqlObjectDatabase.cs)**:
  * Optimized generic and non-generic versions of `FindObjectByKeyImpl` to search for the primary key attribute name using cached metadata instead of querying custom attributes on demand.

---

## 2. Region Updater Loop Optimization

### Problem ("The Why")
Every active world region runs an independent tick loop on a 50ms interval (20 times per second) to update entities (players, NPCs, creatures, objects). 
In the original code, the updater scanned a flat array of 65,000 slots (`Objects.Length`) every tick to check if a slot was occupied and if the object belonged to the region. 
With multiple regions running, this cost millions of array scans per second, even if a region only contained a few dozen active objects.

### Solution ("The What")
We introduced a dynamic active-object tracking list (`_activeObjects`) to keep reference of only the actual entities present in the region. The tick loop and region queries now iterate directly over this collection.

### Files Modified
* **[RegionMgr.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/World/Map/RegionMgr.cs)**:
  * Declared a private tracking list `private readonly List<Object> _activeObjects`.
  * Updated `GenerateOid` to add new entities to `_activeObjects`.
  * Updated `RemoveOldObjects` to remove entities from `_activeObjects` when they leave the region.
  * Optimized `UpdateActors`, `DisposeActors`, `CountObjects`, `GetObjects`, and `GetObjects<T>` to iterate over `_activeObjects` instead of the 65,000-slot `Objects` array, reducing runtime complexity from $O(65000)$ to $O(M)$ where $M$ is the number of active objects in that region.

---

## 3. Network Packet Serialization Memory Optimization

### Problem ("The Why")
Writing primitive datatypes (such as `short`, `int`, `long`, and `float`) to output packets via the `PacketOut` class dynamically called `BitConverter.GetBytes(value)` under the hood. 
Because `BitConverter.GetBytes()` allocates a new heap-allocated `byte[]` on each call, the continuous stream of outgoing game traffic generated immense Garbage Collection (GC) pressure, leading to frequent GC cycles and runtime latency spikes. 

Additionally, a latent bug existed in the `WriteInt16R` method where it wrote loop indexes instead of raw data bytes.

### Solution ("The What")
- We rewrote the integer writing methods (`WriteInt16`, `WriteInt16R`, `WriteInt32`, `WriteInt32R`, `WriteInt64`, `WriteInt64R`) to serialize values directly into the buffer using zero-allocation bit-shifts.
- We defined a stack-allocated union `FloatToIntUnion` to map `float` coordinates/vectors directly to integer bits, allowing `WriteFloat` to serialize floating-point values without heap allocations.
- Fixed the logic in `WriteInt16R` to write actual byte values.

### Files Modified
* **[PacketOut.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/FrameWork/NetWork/Clients/PacketOut.cs)**:
  * Replaced `BitConverter.GetBytes()` calls with zero-allocation bit-shifting and union mapping.

---

## 4. Mono Runtime Memory Allocator Preloading (mimalloc)

### Problem ("The Why")
While managed heap memory is managed by the C# garbage collector, the underlying Mono Virtual Machine (JIT runtime) makes numerous unmanaged C/C++ level allocations (threads, JIT compilation caches, assembly mapping). Utilizing the standard OS memory allocator can result in fragmentation and slower allocation performance under multi-threaded JIT workloads.

### Solution ("The What")
- Installed Microsoft's high-performance memory allocator `mimalloc` (`libmimalloc2.0` and `libmimalloc-dev`) inside the distrobox container environment.
- Configured the startup scripts to preload `libmimalloc.so` before executing Mono.

### Files Modified
* **[container-setup.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/scripts/container-setup.sh)**:
  * Added `libmimalloc2.0` and `libmimalloc-dev` to the list of packages installed automatically during setup.
* **[run-servers.sh](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/scripts/run-servers.sh)**:
  * Configured `start_server` to verify the presence of `libmimalloc.so` and automatically prefix `mono` commands with `LD_PRELOAD=/usr/lib/x86_64-linux-gnu/libmimalloc.so`.

---

## 5. Logging Configuration Stability (Cross-Platform)

### Problem ("The Why")
The logging framework configurations (`NLog.config`) contained hardcoded, Windows-specific path settings for internal logs (`c:\temp\nlog-internal.log`). Additionally, `throwExceptions` was set to `true`, meaning any permission error or directory creation issue on Linux when trying to access Windows-specific file paths would throw an unhandled exception and crash the entire game server.

### Solution ("The What")
- Changed the internal log targets in both configurations to write to a relative folder path (`logs/nlog-internal.log`).
- Set `throwExceptions` to `false` in configuration headers to isolate logging errors and prevent them from crashing the servers.

### Files Modified
* **[WorldServer NLog.config](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/WorldServer/NLog.config)**
* **[Launcher NLog.config](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/Launcher/NLog.config)**

---

## 6. Game Network Latency Optimization (TCP NoDelay)

### Problem ("The Why")
The main game TCP server had `NoDelay` socket setting explicitly set to `false`. This enabled Nagle's algorithm, causing the operating system's networking layer to buffer small packets (such as movement updates or ability casts) for up to 200ms in an attempt to optimize network usage. For real-time multiplayer games, this buffering introduces significant, noticeable network latency (lag).

### Solution ("The What")
- Configured accepted socket connections and listener sockets to set `NoDelay = true` (disabling Nagle's algorithm) to dispatch all game packets instantly.

### Files Modified
* **[TCPManager.cs](file:///run/media/system/NVME_GAME_1/GitHub/ProjectWAR/FrameWork/NetWork/TCPManager.cs)**

---

## Summary of Impact
* **CPU Reduction**: The updater loop drops from scanning 65,000 slots per region tick to iterating only over the exact count of active entities (often $< 100$), drastically lowering baseline server CPU consumption.
* **DB Performance**: Reading and writing records to MySQL is significantly faster and less resource-heavy because metadata mapping bypasses runtime reflection lookups entirely.
* **GC Allocation Reduction**: Allocations from outgoing packets are completely eliminated for primitive serialization, drastically reducing garbage collector pauses and latency spikes.
* **VM Memory Optimization**: The Mono VM benefits from high-performance unmanaged allocations via `LD_PRELOAD` of `mimalloc`, reducing unmanaged allocation latency and VM memory fragmentation.
* **Cross-Platform Stability**: Resolved Windows-specific hardcoded internal paths and NLog exception settings, preventing unexpected file-permission crashes under Linux execution.
* **Reduced Networking Latency**: Disabling Nagle's algorithm (`NoDelay = true`) allows all game status updates to be delivered immediately, dropping in-game ping/latency by up to 200ms.
