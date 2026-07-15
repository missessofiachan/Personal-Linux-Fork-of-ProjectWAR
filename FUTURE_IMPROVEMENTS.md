# ProjectWAR Future Improvements & Road Map

This document outlines a massive list of potential performance, architecture, and code quality improvements for the ProjectWAR server emulator, categorized by ease of implementation ("ease of wins").

---

## Category 1: Easy Wins (Low Effort, High Impact)

### 1. Database Connection String Tuning
* **Goal**: Optimize MySQL connection recycling.
* **Details**: Add explicit connection pooling properties like `Pooling=true;Min Pool Size=5;Max Pool Size=100;Connection Lifetime=60;` in the `<Custom>` connection string tags within `World.xml`, `Lobby.xml`, and `Account.xml`. This prevents connections from closing and reopening frequently under concurrent workloads.
* **Why**: Keeps a hot pool of active MySQL connections ready, reducing handshaking latency during live database operations.

### 2. Skip Verbose Logging for Pings (`F_PING`)
* **Goal**: Eliminate logging overhead on hot network loops.
* **Details**: Currently, logging calls in the network manager print details about every incoming packet. While it ignores `F_PING` debug outputs, checking conditions, string formatting, and tracing should be completely bypassed or compile-gated out in release builds.
* **Why**: Pings occur continuously from all connected clients. Bypassing processing logic on pings saves CPU ticks and minimizes garbage collection pressure from string interpolation.

### 3. Log Level Tuning for Live Deployment
* **Goal**: Reduce IO disk blocking.
* **Details**: Set `LogLevel.Debug` to `false` and adjust `NLog.config` rules to only output `Info`, `Warn`, and `Error` messages in live configurations.
* **Why**: Synchronous or even async trace logging of every game event (e.g. movement, packets) to files is a massive bottleneck on virtualized server disks.

### 4. Dead/Unused Code Cleanup
* **Goal**: Reduce executable footprint and maintenance complexity.
* **Details**: Safely delete the deprecated and unused serialization methods (like `WriteInt16R`, `WriteInt64R` in `CircularBuffer` or helper classes) that were found during static analysis.
* **Why**: Keeps the codebase clean, readable, and aligned with core principles (like KISS/YAGNI).

---

## Category 2: Medium Wins (Medium Effort, High Impact)

### 1. Object Pooling for PacketOut and Byte Buffers (Highly Recommended)
* **Goal**: Eradicate GC pressure from packet building and dynamic buffer resizing.
* **Details**: Although we optimized the serialization logic inside `PacketOut.cs` to write directly via bit-shifts (zero array allocations), the `PacketOut` class instances themselves and their underlying byte array streams are still instantiated on the heap thousands of times a second via `new PacketOut(...)`. Using `ObjectPool<PacketOut>` and a byte array pool (like .NET's `ArrayPool<byte>`) to rent and return these buffers will remove heap allocations entirely.
* **Why**: Eliminates Gen 0 garbage collection cycles during busy gameplay when dozens of packets are dispatched per tick.

### 2. Database Query Profiling and Indexing
* **Goal**: Drastically reduce startup database load times.
* **Details**: Profile slow-loading tables during WorldServer startup (such as `creature_items` which takes ~3 seconds to load 63k rows, and `gameobject_spawns`). Add composite indexes where lookup criteria are matching, or rewrite queries to select only the required fields.
* **Why**: Speeds up database parsing and reduces the boot time of the Lobby and World servers.

### 3. Replace LINQ Queries on Hot Ticks
* **Goal**: Remove hidden allocations on tick loops.
* **Details**: Convert LINQ expressions (like `.Where().Select().ToList()`) inside `RegionMgr.cs` or packet broadcasting loops into standard `for` or `foreach` loops with pre-allocated list targets.
* **Why**: LINQ queries allocate enumerators and closures on the heap every time they run. Replacing them on hot paths reduces GC pauses and memory allocations.

### 4. Task & Event Scheduler Optimization
* **Goal**: Prevent tick-time drift and stabilize game timing.
* **Details**: Inspect the WorldServer update loops, async execution tasks, and timer queues to ensure event handlers and game actions are scheduled efficiently. Avoid thread pool starvation by ensuring task execution queues run on dedicated, optimized worker threads.
* **Why**: Ensures consistent server ticks (e.g. maintaining the 50ms region update interval) even under heavy server loads, avoiding visual rubberbanding and action delays for players.

---

## Category 3: Hard Wins (High Effort, Massive Architectural Gains)

### 1. Migrate to Modern .NET (.NET 8 / .NET 9)
* **Goal**: Modernize the architecture and run natively on Linux.
* **Details**: Upgrade the entire solution from `.NET Framework 4.8` to modern `.NET 8` or `.NET 9`. 
* **Why**: 
  - Eliminates the need for the old Mono runtime on Linux. Modern .NET runs natively with 3x-5x higher throughput.
  - Grants access to advanced JIT optimizations (Dynamic PGO), hardware intrinsics, and allocation-free memory management types like `Span<T>`, `ReadOnlySpan<T>`, and `Memory<T>`.

### 2. Asynchronous Database Access (Async/Await)
* **Goal**: Non-blocking database loading and saving.
* **Details**: Convert the database transaction methods in `ObjectDatabase.cs` and `MysqlObjectDatabase.cs` to use async task operations (e.g. `ExecuteNonQueryAsync`, `ReadAsync`).
* **Why**: Disk IO database saves currently block the main tick thread of the server, causing latency spikes (lag frames) during autosaves. Moving to asynchronous database operations keeps the main thread responsive.

### 3. Spatial Partitioning for AOI (Grid / Quadtree)
* **Goal**: Optimize visibility calculations for high player counts.
* **Details**: Replace the cell-based visibility searches in `RegionMgr.cs` with a dynamic 2D Spatial Grid or a Quadtree to track players and NPC ranges.
* **Why**: As entity counts increase, calculating distances between all entities becomes extremely slow ($O(N^2)$). Spatial partitioning narrows checks down to immediately adjacent nodes, enabling support for large battles without tick delays.
