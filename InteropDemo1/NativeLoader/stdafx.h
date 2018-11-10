// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>


#include <vector>
#include <iostream>
#include <cstddef> // std::byte

struct holder
{
	std::vector<std::byte> buffer;

	holder()
	{
		std::cout << "holder ctor" << std::endl;
		OutputDebugString(L"Interop2NativeProducer: globals created");
	}

	~holder()
	{
		std::cout << "holder dtor" << std::endl;
		OutputDebugString(L"Interop2NativeProducer: globals destroyed");
	}
};


#define EXTERN_DLL_EXPORT extern "C" __declspec(dllexport)

// reference additional headers your program requires here
