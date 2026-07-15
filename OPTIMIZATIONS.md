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

## Summary of Impact
* **CPU Reduction**: The updater loop overhead drops from scanning 65,000 slots per region tick to iterating only over the exact count of active entities (often $< 100$), drastically lowering baseline server CPU consumption.
* **DB Performance**: Reading and writing records to MySQL is significantly faster and less resource-heavy because metadata mapping bypasses runtime reflection lookups entirely.
