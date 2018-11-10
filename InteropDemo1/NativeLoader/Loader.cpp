#include "stdafx.h"
#include <vector>
#include <string>
#include <string.h>
#include <cstddef> // std::byte
#include <iostream>
#include <fstream>
#include <iterator>
#include <filesystem>

#include "wavheader.h"

std::string getErrorMessage()
{
	char errmsg[1024];
	strerror_s(errmsg, errno);
	return std::string(errmsg);
}

std::vector<std::byte> readFile(const std::string& filename)
{
	std::ifstream file(filename, std::ios::binary | std::ios::ate);	// open and seek to the end
	if (!file) throw std::runtime_error(filename + " - " + getErrorMessage());

	// compute the file size
	auto endpos = file.tellg();
	file.seekg(0, std::ios::beg);
	auto size = std::size_t(endpos - file.tellg());
	if (size == 0) return {};

	// load all the content into the buffer
	std::vector<std::byte> buffer(size);
	if (!file.read((char*)buffer.data(), buffer.size()))
		throw std::runtime_error(filename + " - " + getErrorMessage());

	return buffer;
}

EXTERN_DLL_EXPORT int getLibraryVersion()
{
	return 1;
}

EXTERN_DLL_EXPORT void* Prepare(const char* filename)
{
	std::cout << "Prepare started" << std::endl;
	std::string f(filename);
	auto handle = new holder();
	handle->buffer = readFile(f);
	return handle;
}

EXTERN_DLL_EXPORT void Free(void* handle)
{
	std::cout << "Free started" << std::endl;
	if (handle == nullptr) return;
	auto h = static_cast<holder*>(handle);
	h->buffer = {};
	delete h;
}

EXTERN_DLL_EXPORT void Read(void* handle, char** data, size_t* length)
{
	//std::cout << "Read started" << std::endl;
	if (handle == nullptr) return;
	if (data == nullptr) return;
	if (length == nullptr) return;

	auto h = static_cast<holder*>(handle);
	*length = h->buffer.size();

	if (*length == 0)
	{
		*data = 0;
		return;
	}

	*data = (char*)&h->buffer[0];
}


EXTERN_DLL_EXPORT wavheader ReadWavHeader(void* handle)
{
	//std::cout << "ReadWavHeader started" << std::endl;
	if (handle == nullptr) return wavheader();

	auto h = static_cast<holder*>(handle);

	return *(wavheader*)&h->buffer[0];
}

EXTERN_DLL_EXPORT void* ReadUnsafe(void* handle)
{
	//std::cout << "ReadWavHeader started" << std::endl;
	if (handle == nullptr) return nullptr;

	auto h = static_cast<holder*>(handle);

	return &h->buffer[0];
}
