#include "Platform.h"
#include "ZoneManager.h"

static ZoneManager* _manager = nullptr;
extern "C"
{
#ifdef _WIN32
	__declspec(dllexport) int __stdcall Pin(int zoneID, int x, int y)
#else
	int Pin(int zoneID, int x, int y)
#endif
	{
		if (_manager == nullptr)
		{
			printf("WarZone -- Error Must call InitZones() first!");
			return -1;
		}

		return _manager->Pin(zoneID, x, y);
	}

#ifdef _WIN32
	__declspec(dllexport) void __stdcall InitZones(const char* path, int triCount)
#else
	void InitZones(const char* path, int triCount)
#endif
	{
		if (_manager != nullptr)
			delete _manager;

		_manager = new ZoneManager(path, triCount);
	}

#ifdef _WIN32
	__declspec(dllexport) void __stdcall LoadZone(int zoneID)
#else
	void LoadZone(int zoneID)
#endif
	{
		if (_manager == nullptr)
		{
			printf("WarZone -- Error Must call InitZones() first!");
			return;
		}

		_manager->LoadZone(zoneID);
	}

#ifdef _WIN32
	__declspec(dllexport) void __stdcall UnLoadZone(int zoneID)
#else
	void UnLoadZone(int zoneID)
#endif
	{
		if (_manager == nullptr)
			return;

		_manager->UnloadZone(zoneID);
	}

#ifdef _WIN32
	__declspec(dllexport) OcclusionResult __stdcall SegmentIntersect(int zoneIDA, int zoneIDB,
		float originX, float originY, float originZ,
		float targetX, float targetY, float targetZ,
		bool terrain, bool normalTest, int triCount, OcclussionInfo* result)
#else
	OcclusionResult SegmentIntersect(int zoneIDA, int zoneIDB,
		float originX, float originY, float originZ,
		float targetX, float targetY, float targetZ,
		bool terrain, bool normalTest, int triCount, OcclussionInfo* result)
#endif
	{
		if (_manager == nullptr)
			return OcclusionResult::NotLoaded;

		return _manager->SegmentIntersect(zoneIDA, zoneIDB, originX, originY, originZ,
			targetX, targetY, targetZ, terrain, normalTest, triCount, result);
	}

#ifdef _WIN32
	__declspec(dllexport) bool __stdcall TerrainIntersect(int zoneIDA, int zoneIDB,
		float originX, float originY, float originZ,
		float targetX, float targetY, float targetZ, int triCount, OcclussionInfo* result)
#else
	bool TerrainIntersect(int zoneIDA, int zoneIDB,
		float originX, float originY, float originZ,
		float targetX, float targetY, float targetZ, int triCount, OcclussionInfo* result)
#endif
	{
		if (_manager == nullptr)
			return false;

		return _manager->TerrainIntersect(zoneIDA, zoneIDB, originX, originY, originZ,
			targetX, targetY, targetZ, triCount, result);
	}

#ifdef _WIN32
	__declspec(dllexport) bool __stdcall SetFixtureVisible(int zoneID, uint32_t uniqueID, uint8_t instanceID, bool visible)
#else
	bool SetFixtureVisible(int zoneID, uint32_t uniqueID, uint8_t instanceID, bool visible)
#endif
	{
		if (_manager == nullptr)
			return false;

		return _manager->SetFixtureVisible(zoneID, uniqueID, instanceID, visible);
	}

#ifdef _WIN32
	__declspec(dllexport) bool __stdcall GetFixtureVisible(int zoneID, uint32_t uniqueID, uint8_t instanceID)
#else
	bool GetFixtureVisible(int zoneID, uint32_t uniqueID, uint8_t instanceID)
#endif
	{
		if (_manager == nullptr)
			return false;

		return _manager->GetFixtureVisible(zoneID, uniqueID, instanceID);
	}

#ifdef _WIN32
	__declspec(dllexport) int __stdcall GetFixtureCount(int zoneID)
#else
	int GetFixtureCount(int zoneID)
#endif
	{
		if (_manager == nullptr)
			return 0;

		return _manager->GetFixtureCount(zoneID);
	}

#ifdef _WIN32
	__declspec(dllexport) bool __stdcall GetFixtureInfo(int zoneID, int index, FixtureInfo* info)
#else
	bool GetFixtureInfo(int zoneID, int index, FixtureInfo* info)
#endif
	{
		if (_manager == nullptr)
			return false;

		return _manager->GetFixtureInfo(zoneID, index, info);
	}
}
