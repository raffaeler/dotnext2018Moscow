using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace InteropConsole
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WavHeader
    {
        /// <summary>
        /// Offset 0: RIFF marker
        /// </summary>
        public fixed byte ChunkID[4];

        /// <summary>
        /// Offset 4
        /// </summary>
        public Int32 ChunkSize;

        /// <summary>
        /// Offset 8
        /// </summary>
        public fixed byte Format[4];

        /// <summary>
        /// Offset 12
        /// </summary>
        public fixed byte SubChunk1ID[4];

        /// <summary>
        /// Offset 16
        /// </summary>
        public Int32 SubChunk1Size;

        /// <summary>
        /// Offset 20
        /// </summary>
        public Int16 AudioFormat;

        /// <summary>
        /// Offset 22
        /// </summary>
        public Int16 NumChannels;

        /// <summary>
        /// Offset 24
        /// </summary>
        public Int32 SampleRate;

        /// <summary>
        /// Offset 28
        /// </summary>
        public Int32 ByteRate;

        /// <summary>
        /// Offset 32
        /// </summary>
        public Int16 BlockAlign;

        /// <summary>
        /// Offset 34
        /// </summary>
        public Int16 BitsPerSample;
    }
}
