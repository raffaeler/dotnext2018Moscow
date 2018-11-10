#pragma once

#include <stdint.h>

// there is no packing in the file format
#pragma pack(push, 1)
struct wavheader
{
	char chunkid[4];

	int32_t chunksize;

	char format[4];

	char subchunk1id[4];

	int32_t subchunk1size;

	int16_t audioformat;

	int16_t numchannels;

	int32_t samplerate;

	int32_t byterate;

	int16_t blockalign;

	int16_t bitspersample;
};

#pragma pack(pop)