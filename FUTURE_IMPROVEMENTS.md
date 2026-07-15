# ProjectWAR Future Improvements & Road Map

This document outlines a comprehensive architecture and optimization roadmap for the ProjectWAR server emulator. The objectives are categorized by implementation complexity, focusing on maximizing throughput, eliminating memory allocations, and unlocking native multi-core Linux efficiency.

---

## Category 1: Easy Wins (Low Effort, High Impact)

### 1. Database Connection String Tuning

* **Goal:** Optimize MySQL/MariaDB connection recycling and reduce handshake overhead.
* **Details:** Append explicit connection pooling properties inside the `<Custom>` connection string tags within `World.xml`, `Lobby.xml`, and `Account.xml`:
```text
Pooling=true;Min Pool Size=10;Max Pool Size=100;Connection Lifetime=60;SslMode=None;

```


* **Why:** Eliminates the TCP connection and TLS handshake latency on local Docker/Distrobox loopback networks by keeping a warm pool of authenticated database channels ready for immediate thread reuse.

### 2. Zero-Allocation Ping (`F_PING`) & Hot Path Log Gating

* **Goal:** Banish CPU and string formatting overhead from telemetry network loops.
* **Details:** Bypassing packet logging outputs is insufficient if string arguments are still evaluated. Wrap all hot path logs in explicit conditional checks or use high-performance compile-time logging source generators:
```csharp
// Avoid: Interpolation allocates heap strings regardless of log state
Log.Debug($"Packet received: {packet.Id}"); 

// Implement: Zero allocation when the log level is filtered out
if (Log.IsDebugEnabled) Log.Debug("Packet received: {0}", packet.Id);

```


* **Why:** Heartbeats and pings hit the network thread pool dozens of times per second per player. Bypassing string generation prevents unnecessary Gen 0 heap garbage collection churn.

### 3. Enum Boxing Prevention on Bitwise Flags

* **Goal:** Eliminate silent type-boxing during state and status evaluations.
* **Details:** Scan update loops for references to `.HasFlag()` on enum fields (e.g., player combat states or NPC flags). Replace them with raw bitwise integer masks:
```csharp
// Replace: (player.States.HasFlag(CombatStates.InCombat))
// Implement:
if ((player.States & CombatStates.InCombat) != 0) { ... }

```


* **Why:** In older runtime environments, `.HasFlag` implicitly boxes the enum value onto the managed heap. Under heavy combat loops, this creates hundreds of thousands of micro-allocations.

### 4. Collection Capacity Pre-allocation

* **Goal:** Eradicate dynamic array resizing penalties during data hydration.
* **Details:** Audit server initialization paths (like `creature_items`, `gameobject_spawns`, and spell definitions). Explicitly supply the expected record count to list and dictionary constructors instead of instantiating them empty:
```csharp
// Implement:
var spawns = new List<GameObjectSpawn>(dbCount);

```


* **Why:** Default collections resize by doubling their internal arrays whenever limits are breached. Pre-sizing collections drops memory re-allocations and data-copying operations during server boot to exactly zero.

---

## Category 2: Medium Wins (Medium Effort, High Impact)

### 1. Object Pooling for Packet Containers and Byte Buffers

* **Goal:** Achieve a completely allocation-free network broadcasting pipeline.
* **Details:** Transition the network ingestion layer to rent and return underlying byte streams via .NET's native `ArrayPool<byte>.Shared`. Wrap `PacketOut` allocation routines inside an `ObjectPool<PacketOut>`, ensuring strict lifecycle management via `try...finally` scopes:
```csharp
byte[] buffer = ArrayPool<byte>.Shared.Rent(packetSize);
try {
    // Build and dispatch packet using the rented slice
} finally {
    ArrayPool<byte>.Shared.Return(buffer);
}

```


* **Why:** Eliminates the generational heap churn caused by creating thousands of short-lived packet objects during global map broadcasts (e.g., positional updates in RvR scenarios).

### 2. High-Performance Packet Parsing via `Span<T>`

* **Goal:** Eradicate data copying on incoming client network buffers.
* **Details:** Refactor the packet decryption and interpretation routines inside `TCPManager` to read byte structures via `ReadOnlySpan<byte>` and `Memory<T>` instead of copying raw payloads into newly allocated byte arrays.
* **Why:** Allows the server to slice, interpret, and unpack client data directly from the active socket socket buffers, lowering cache-miss frequencies at the network layer.

### 3. Compiled Delegate Packet Dispatcher

* **Goal:** Replace reflection-based packet routing with lightning-fast compiled code execution.
* **Details:** Replace the startup network reflection routines that map packet opcodes to processing methods using `MethodInfo.Invoke`. Transition the router to use C# Expression Trees to compile packet handler methods into strongly-typed delegates (`Action<Client, PacketIn>`) at server startup.
* **Why:** Dynamic reflection invocation adds dramatic execution overhead on network threads. Compiled expression delegates invoke handlers at the speed of native direct method calls.

### 4. Database Query Index Profiling

* **Goal:** Shave startup loading bottlenecks down to a fraction of their current times.
* **Details:** Identify tables causing multi-second delays during server boot phases (such as `creature_items` or world geometry definitions). Apply targeted composite database indexes on frequently queried combinations (e.g., `creature_id` + `slot_id`).
* **Why:** Minimizes disk I/O wait states inside the MariaDB host, accelerating server reboots and cluster recovery processes.

### 5. Loop Optimization & LINQ Elimination on Hot Path Ticks

* **Goal:** Eradicate background Garbage Collector latency spikes during real-time updates.
* **Details:** Systematically convert all LINQ query chains (`.Where()`, `.Select()`, `.Any()`) executed within `RegionMgr.cs`, AI decision loops, and threat table evaluations into primitive `for` index arrays.
* **Why:** LINQ allocation mechanics instantiate closure contexts and enumerator trackers on every cycle. Replacing them with index-driven iteration stops stop-the-world GC pauses from causing positional rubber-banding.

---

## Category 3: Hard Wins (High Effort, Massive Architectural Gains)

### 1. Native Migration to Modern Linux .NET (.NET 9 / .NET 10)

* **Goal:** Eliminate runtime emulation overhead and maximize JIT optimization capability.
* **Details:** Port the entire solution configuration stack away from legacy `.NET Framework 4.8` targeting native Linux `.NET 9/10` architectures. This completely replaces the Mono JIT virtualization layers with the modern CoreCLR engine.
* **Why:**
* Unlocks **Dynamic PGO (Profile-Guided Optimization)**, allowing the engine to dynamically rewrite machine code instructions for highly executed pathways.
* Grants immediate access to allocation-free core structural elements (`Span<T>`, `System.Threading.Channels`).
* Yields an immediate 3x to 5x boost in overall execution throughput under multi-threaded server workloads.



### 2. Source-Generated Interop Layer (`LibraryImport`) for `libWarZone.so`

* **Goal:** Eradicate marshaling overhead between the C# WorldServer and the C++ physics engine.
* **Details:** Once running on modern .NET, transition the external physics links inside `LinuxPort.h` and the C# server wrapper from runtime `[DllImport]` configurations to compile-time source-generated `[LibraryImport]` structures.
* **Why:** Legacy P/Invoke relies on runtime stub generation to map memory structures between managed and unmanaged code boundaries. Source generation constructs exact binary layout translation stubs during compilation, reducing cross-language execution friction.

### 3. Flat 2D Spatial Grid Partitioning for Area of Interest (AOI)

* **Goal:** Reduce visibility checking complexity from a bottlenecked quadratic scale down to linear performance.
* **Details:** Abandon old proximity evaluation logic in favor of a flat, fixed-size 2D spatial grid partitioning architecture. Divide the world map into geometric blocks matching the max visibility distance of a client player. Moving entities register and unregister dynamically inside local array buckets:
```text
Object Visibility Search Space:
[ Old Design: O(N²) Global Scan ] -> [ New Design: O(1) Bucket Indexing ]

```


* **Why:** Avoids checking distances across unrelated entities across the zone. When checking for nearby targets, the engine only queries the current cell and its immediate 8 neighbors, keeping the computation cost flat even during massive, hundreds-of-players RvR sieges.

### 4. Decoupled Threading Architecture: Single-Threaded Region Fibers

* **Goal:** Strip out thread locking contention across the game simulation layer.
* **Details:** Isolate every active `RegionMgr` zone instance onto its own dedicated, loop-driven execution thread (a Fiber execution model). All data manipulation occurring inside that zone runs completely sequentially. Cross-region events (such as character map transfers or global whispers) are handled using lock-free concurrent message passing queues (`ConcurrentQueue<T>`).
* **Why:** Eliminates the need for chaotic, scattered `lock()` statements across player, NPC, and item collections. Removing lock boundaries guarantees the CPU can run zone calculations at full speed without waiting on thread synchronization blocks.

### 5. Fully Asynchronous I/O Database Hydration (Async/Await)

* **Goal:** Ensure database read/write actions never stall the global game loop.
* **Details:** Re-engineer the underlying operations in `ObjectDatabase.cs` to leverage async task paradigms natively (`ExecuteNonQueryAsync`, `ReadAsync`).
* **Why:** Traditional blocking database calls force the executing game loop thread to freeze while waiting for disk input/output confirmation. Asynchronous database management cleanly unloads data persistence operations onto system worker channels, ensuring the active game world ticks perfectly at its designated 50ms interval.

### 6. Lock-Free Static Game Data Splitting (Flyweight Design)

* **Goal:** Safe, lock-free access to global game definitions across multiple server zones.
* **Details:** Decouple structural runtime states (e.g., current entity health, positions) from immutable master data templates (e.g., base ability damage, item names, quest text). Pack static master data into immutable, read-only structures initialized once at boot.
* **Why:** Since static game data never changes during runtime, any thread or region loop can read it simultaneously without locks or synchronization primitives. This significantly improves L1/L2 cache efficiency across the CPU core topology.
