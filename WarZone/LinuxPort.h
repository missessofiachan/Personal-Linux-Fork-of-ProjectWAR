#pragma once
#ifndef _WIN32
#include <stdio.h>
#include <string.h> // for memset
#define _fseeki64 fseeko
#define _ftelli64 ftello
#endif
