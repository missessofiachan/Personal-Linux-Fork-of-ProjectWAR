#pragma once
#include "Platform.h"
#include <string>
#include <cmath>

#ifdef _WIN32
#include <mmsystem.h>
#include <psapi.h>
#include <windows.h>
#else
#include <sys/stat.h>
#include <unistd.h>
#endif

class Util
{
public:
	static inline double GetDistance(glm::vec3 locA, glm::vec3 locB)
	{
		double deltaX = (int)locB.x - (int)locA.x;
		double deltaY = (int)locB.y - (int)locA.y;
		double deltaZ = (int)locB.z - (int)locA.z;

		return sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
	}

	static inline glm::vec3 Lerp(glm::vec3 a, glm::vec3 b, double t)
	{
		return glm::vec3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	static std::string PathAppend(const std::string& p1, const std::string& p2)
	{
		char sep = '/';
		std::string tmp = p1;

#ifdef _WIN32
		sep = '\\';
#endif

		if (p1.empty() || p1[p1.length() - 1] != sep) { // Need to add a
			tmp += sep;                // path separator
			return(tmp + p2);
		}
		else
			return(p1 + p2);
	}

	static int64_t FileSize(std::string name)
	{
#ifdef _WIN32
		std::wstring wc(name.begin(), name.end());
		auto result = FileSize(wc.c_str());
		return result;
#else
		struct stat stat_buf;
		int rc = stat(name.c_str(), &stat_buf);
		return rc == 0 ? stat_buf.st_size : -1;
#endif
	}

#ifdef _WIN32
	static int64_t FileSize(const wchar_t* name)
	{
		HANDLE hFile = CreateFile(name, GENERIC_READ,
			FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL, NULL);
		if (hFile == INVALID_HANDLE_VALUE)
			return -1;

		LARGE_INTEGER size;
		if (!GetFileSizeEx(hFile, &size))
		{
			CloseHandle(hFile);
			return -1;
		}

		CloseHandle(hFile);
		return size.QuadPart;
	}
#endif

	static void PrintMemoryInfo()
	{
#ifdef _WIN32
		PrintMemoryInfo(GetCurrentProcessId());
#endif
	}

#ifdef _WIN32
	static void PrintMemoryInfo(DWORD processID)
	{
		HANDLE hProcess;
		PROCESS_MEMORY_COUNTERS pmc;

		printf("\nProcess ID: %u\n", processID);

		hProcess = OpenProcess(PROCESS_QUERY_INFORMATION |
			PROCESS_VM_READ,
			FALSE, processID);
		if (NULL == hProcess)
			return;

		if (GetProcessMemoryInfo(hProcess, &pmc, sizeof(pmc)))
		{
			printf("\tWorkingSetSize: %dMB\r\n", (int)(pmc.WorkingSetSize / 1024 / 1024));
		}

		CloseHandle(hProcess);
	}
#endif
};
