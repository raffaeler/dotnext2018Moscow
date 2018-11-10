using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

// Runtime = .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT; GC = Concurrent Workstation
// x64, Loop = 1_000
//              Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto | 216.1 us | 1.8101 us | 1.6932 us |           - |           - |           - |                   - |
//       PInvokeManual | 509.5 us | 8.3662 us | 7.8257 us |     12.6953 |           - |           - |             56000 B |
// PInvokeSingleFields | 358.1 us | 2.8329 us | 2.5113 us |           - |           - |           - |                   - |
//      SpanByteIntPtr | 328.8 us | 6.4680 us | 7.9433 us |           - |           - |           - |                   - |
//            SpanByte | 199.6 us | 1.7883 us | 1.6727 us |           - |           - |           - |                   - |
//          SpanStruct | 201.2 us | 2.4763 us | 2.3163 us |           - |           - |           - |                   - |
//    SpanStructAndRef | 201.9 us | 4.3154 us | 4.9696 us |           - |           - |           - |                   - |
//     SpanByteAndCast | 199.7 us | 0.9525 us | 0.7954 us |           - |           - |           - |                   - |
//     SpanByteAndRead | 200.0 us | 0.8209 us | 0.7678 us |           - |           - |           - |                   - |
//          UnsafeRead | 201.5 us | 2.1447 us | 1.9012 us |           - |           - |           - |                   - |
//         UnsafeAsRef | 200.9 us | 2.1558 us | 1.9111 us |           - |           - |           - |                   - |


// Runtime = .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT; GC = Concurrent Workstation
// x64, Loop = 1_000_000
//              Method |     Mean |    Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |---------:|---------:|----------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto | 215.1 ms | 2.325 ms |  2.061 ms |           - |           - |           - |                   - |
//       PInvokeManual | 511.4 ms | 5.394 ms |  4.782 ms |  13000.0000 |           - |           - |          56000000 B |
// PInvokeSingleFields | 367.9 ms | 7.171 ms | 11.980 ms |           - |           - |           - |                   - |
//      SpanByteIntPtr | 322.2 ms | 2.938 ms |  2.605 ms |           - |           - |           - |                   - |
//            SpanByte | 199.2 ms | 1.947 ms |  1.520 ms |           - |           - |           - |                   - |
//          SpanStruct | 201.8 ms | 3.166 ms |  2.962 ms |           - |           - |           - |                   - |
//    SpanStructAndRef | 201.3 ms | 2.610 ms |  2.314 ms |           - |           - |           - |                   - |
//     SpanByteAndCast | 201.6 ms | 3.278 ms |  3.066 ms |           - |           - |           - |                   - |
//     SpanByteAndRead | 201.8 ms | 3.116 ms |  2.762 ms |           - |           - |           - |                   - |
//          UnsafeRead | 202.5 ms | 3.224 ms |  3.016 ms |           - |           - |           - |                   - |
//         UnsafeAsRef | 204.0 ms | 3.970 ms |  4.875 ms |           - |           - |           - |                   - |



namespace InteropConsole
{
    [MemoryDiagnoser]
    public class TestNative : IDisposable
    {
        private const int Loop = 1_000;
        //private const int Loop = 1_000_000;

        private NativeInterop _native;

        public TestNative()
        {
            _native = new NativeInterop(@"assets\dsp_demo_sample.wav");
        }


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
            for (int i = 0; i < Loop; i++)
            {
                _native.Read(out IntPtr data, out int length);
                var spanByte = new Span<byte>(data.ToPointer(), sizeof(WavHeader));
            }
        }

        [Benchmark]
        public unsafe void SpanByte()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var spanByte = new Span<byte>(ptr, sizeof(WavHeader));
            }
        }

        [Benchmark]
        public unsafe void SpanStruct()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var spanWavHeader = new Span<WavHeader>(ptr, 1);
            }
        }

        [Benchmark]
        public unsafe void SpanStructAndRef()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var spanWavHeader = new Span<WavHeader>(ptr, 1);
                ref var refSpanWavHeader = ref MemoryMarshal.GetReference<WavHeader>(spanWavHeader);
            }
        }

        [Benchmark]
        public unsafe void SpanByteAndCast()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var spanByte = new Span<byte>(ptr, sizeof(WavHeader));
                var wavheaderx1 = MemoryMarshal.Cast<byte, WavHeader>(spanByte);
            }
        }

        [Benchmark]
        public unsafe void SpanByteAndRead()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var spanByte = new Span<byte>(ptr, sizeof(WavHeader));
                var wavheader2 = MemoryMarshal.Read<WavHeader>(spanByte);
            }
        }

        [Benchmark]
        public unsafe void UnsafeRead()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                var wavheader = Unsafe.Read<WavHeader>(ptr);
            }
        }

        [Benchmark]
        public unsafe void UnsafeAsRef()
        {
            for (int i = 0; i < Loop; i++)
            {
                var ptr = _native.ReadUnsafe();
                ref var wavheader = ref Unsafe.AsRef<WavHeader>(ptr);
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
