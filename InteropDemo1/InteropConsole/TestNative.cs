using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

// * Summary *
//
//BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
//Intel Core i7-6700 CPU 3.40GHz(Skylake), 1 CPU, 8 logical and 4 physical cores
//.NET Core SDK = 2.1.500
//
// [Host]     : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
//  DefaultJob : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
//
//              Method |    Loop |           Mean |         Error |        StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |-------- |---------------:|--------------:|--------------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto |    1000 |      12.617 us |     0.1081 us |     0.0958 us |           - |           - |           - |                   - |
//       PInvokeManual |    1000 |     188.891 us |     3.7755 us |     5.1679 us |     13.1836 |           - |           - |             56000 B |
// PInvokeSingleFields |    1000 |      42.619 us |     0.4199 us |     0.3927 us |           - |           - |           - |                   - |
//      SpanByteIntPtr |    1000 |      10.438 us |     0.1345 us |     0.1259 us |           - |           - |           - |                   - |
//      UnsafeBaseline |    1000 |       3.077 us |     0.0465 us |     0.0435 us |           - |           - |           - |                   - |
//            SpanByte |    1000 |       3.080 us |     0.0560 us |     0.0496 us |           - |           - |           - |                   - |
//          SpanStruct |    1000 |       3.125 us |     0.0597 us |     0.0613 us |           - |           - |           - |                   - |
//    SpanStructAndRef |    1000 |       2.817 us |     0.0560 us |     0.0622 us |           - |           - |           - |                   - |
//     SpanByteAndCast |    1000 |       3.097 us |     0.0338 us |     0.0300 us |           - |           - |           - |                   - |
//     SpanByteAndRead |    1000 |       3.407 us |     0.0447 us |     0.0418 us |           - |           - |           - |                   - |
//          UnsafeRead |    1000 |       3.690 us |     0.0386 us |     0.0361 us |           - |           - |           - |                   - |
//         UnsafeAsRef |    1000 |       4.371 us |     0.0723 us |     0.0677 us |           - |           - |           - |                   - |
//-------------------- |-------- |---------------:|--------------:|--------------:|------------:|------------:|------------:|--------------------:|
//              Method |    Loop |           Mean |         Error |        StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |-------- |---------------:|--------------:|--------------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto | 1000000 |  12,612.498 us |   156.1034 us |   146.0192 us |           - |           - |           - |                   - |
//       PInvokeManual | 1000000 | 181,735.211 us | 3,490.3490 us | 3,879.5137 us |  13333.3333 |           - |           - |          56000000 B |
// PInvokeSingleFields | 1000000 |  43,325.783 us |   835.8621 us | 1,225.1958 us |           - |           - |           - |                   - |
//      SpanByteIntPtr | 1000000 |  10,522.530 us |   185.2849 us |   173.3156 us |           - |           - |           - |                   - |
//      UnsafeBaseline | 1000000 |   3,247.008 us |    61.5959 us |    60.4954 us |           - |           - |           - |                   - |
//            SpanByte | 1000000 |   3,273.589 us |    68.9602 us |    92.0599 us |           - |           - |           - |                   - |
//          SpanStruct | 1000000 |   3,253.136 us |    43.8460 us |    41.0136 us |           - |           - |           - |                   - |
//    SpanStructAndRef | 1000000 |   2,786.450 us |    55.5301 us |    51.9429 us |           - |           - |           - |                   - |
//     SpanByteAndCast | 1000000 |   3,119.930 us |    60.9927 us |    89.4023 us |           - |           - |           - |                   - |
//     SpanByteAndRead | 1000000 |   3,374.622 us |    31.1955 us |    26.0497 us |           - |           - |           - |                   - |
//          UnsafeRead | 1000000 |   3,793.350 us |    72.6031 us |    86.4288 us |           - |           - |           - |                   - |
//         UnsafeAsRef | 1000000 |   4,453.932 us |    88.8326 us |    87.2455 us |           - |           - |           - |                   - |


namespace InteropConsole
{
    [MemoryDiagnoser]
    public class TestNative : IDisposable
    {
        private NativeInterop _native;

        public TestNative()
        {
            _native = new NativeInterop(@"assets\dsp_demo_sample.wav");
        }

        [Params(1_000, 1_000_000)]
        public int Loop { get; set; }

        public void Dispose()
        {
            _native.Dispose();
        }

        [Benchmark]
        public void PInvokeAuto()
        {
            for (int i = 0; i < Loop; i++)
            {
                WavHeader wavHeader3 = _native.ReadWavHeader();
            }
        }

        [Benchmark]
        public void PInvokeManual()
        {
            for (int i = 0; i < Loop; i++)
            {
                _native.Read(out IntPtr data, out int length);
                WavHeader wavheader1 = Marshal.PtrToStructure<WavHeader>(data);
            }
        }

        [Benchmark]
        public unsafe void PInvokeSingleFields()
        {
            for (int i = 0; i < Loop; i++)
            {
                _native.Read(out IntPtr data, out int length);
                WavHeader wavheader1;
                wavheader1.ChunkID[0] = Marshal.ReadByte(data, 0);
                wavheader1.ChunkID[1] = Marshal.ReadByte(data, 1);
                wavheader1.ChunkID[2] = Marshal.ReadByte(data, 2);
                wavheader1.ChunkID[3] = Marshal.ReadByte(data, 3);

                wavheader1.ChunkSize = Marshal.ReadInt32(data, 4);
                wavheader1.Format[0] = Marshal.ReadByte(data, 8);
                wavheader1.Format[1] = Marshal.ReadByte(data, 9);
                wavheader1.Format[2] = Marshal.ReadByte(data, 10);
                wavheader1.Format[3] = Marshal.ReadByte(data, 11);

                wavheader1.SubChunk1ID[0] = Marshal.ReadByte(data, 12);
                wavheader1.SubChunk1ID[1] = Marshal.ReadByte(data, 13);
                wavheader1.SubChunk1ID[2] = Marshal.ReadByte(data, 14);
                wavheader1.SubChunk1ID[3] = Marshal.ReadByte(data, 15);

                wavheader1.SubChunk1Size = Marshal.ReadInt16(data, 16);
                wavheader1.AudioFormat = Marshal.ReadInt16(data, 20);
                wavheader1.NumChannels = Marshal.ReadInt16(data, 22);
                wavheader1.SampleRate = Marshal.ReadInt32(data, 24);
                wavheader1.ByteRate = Marshal.ReadInt32(data, 28);

                wavheader1.BlockAlign = Marshal.ReadInt16(data, 32);
                wavheader1.BitsPerSample = Marshal.ReadInt16(data, 34);
            }
        }

        [Benchmark]
        public unsafe void SpanByteIntPtr()
        {
            int wavHeaderLength = sizeof(WavHeader);
            for (int i = 0; i < Loop; i++)
            {
                _native.Read(out IntPtr data, out int length);
                Span<byte> spanByte = new Span<byte>(data.ToPointer(), wavHeaderLength);
            }
        }

        [Benchmark]
        public unsafe void UnsafeBaseline()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
            }
        }

        [Benchmark]
        public unsafe void SpanByte()
        {
            int wavHeaderLength = sizeof(WavHeader);
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                Span<byte> spanByte = new Span<byte>(ptr, wavHeaderLength);
            }
        }

        [Benchmark]
        public unsafe void SpanStruct()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                Span<WavHeader> spanWavHeader = new Span<WavHeader>(ptr, 1);
            }
        }

        [Benchmark]
        public unsafe void SpanStructAndRef()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                Span<WavHeader> spanWavHeader = new Span<WavHeader>(ptr, 1);
                ref WavHeader refWavHeader = ref MemoryMarshal.GetReference<WavHeader>(spanWavHeader);

                // From MemoryMarshal sources:
                // public static ref T GetReference<T>(Span<T> span) => ref span._pointer.Value;
            }
        }

        [Benchmark]
        public unsafe void SpanByteAndCast()
        {
            int wavHeaderLength = sizeof(WavHeader);
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                Span<byte> spanByte = new Span<byte>(ptr, wavHeaderLength);
                Span<WavHeader> wavheader = MemoryMarshal.Cast<byte, WavHeader>(spanByte);
            }
        }

        [Benchmark]
        public unsafe void SpanByteAndRead()
        {
            int wavHeaderLength = sizeof(WavHeader);
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                Span<byte> spanByte = new Span<byte>(ptr, wavHeaderLength);
                WavHeader wavheader = MemoryMarshal.Read<WavHeader>(spanByte);
            }
        }

        [Benchmark]
        public unsafe void UnsafeRead()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                WavHeader wavheader = Unsafe.Read<WavHeader>(ptr);
            }
        }

        [Benchmark]
        public unsafe void UnsafeAsRef()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte* ptr = _native.ReadUnsafe();
                ref WavHeader wavheader = ref Unsafe.AsRef<WavHeader>(ptr);
            }
        }


        //[Benchmark]
        //public unsafe void x()
        //{
        //    for (int i = 0; i < Loop; i++)
        //    {

        //    }
        //}
    }
}
