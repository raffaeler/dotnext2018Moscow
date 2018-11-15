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


//              Method |       Mean |     Error |    StdDev |     Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |-----------:|----------:|----------:|-----------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto |  12.788 us | 0.0978 us | 0.0915 us |  12.765 us |           - |           - |           - |                   - |
//       PInvokeManual | 185.007 us | 2.9501 us | 2.6152 us | 184.456 us |     13.1836 |           - |           - |             56000 B |
// PInvokeSingleFields |  42.928 us | 0.1903 us | 0.1589 us |  43.020 us |           - |           - |           - |                   - |
//      SpanByteIntPtr |  10.743 us | 0.2147 us | 0.2556 us |  10.677 us |           - |           - |           - |                   - |
//            SpanByte |   3.043 us | 0.0785 us | 0.2163 us |   2.944 us |           - |           - |           - |                   - |
//          SpanStruct |   3.067 us | 0.0801 us | 0.2205 us |   2.966 us |           - |           - |           - |                   - |
//    SpanStructAndRef |   3.217 us | 0.0470 us | 0.0393 us |   3.222 us |           - |           - |           - |                   - |
//     SpanByteAndCast |   3.240 us | 0.0616 us | 0.0576 us |   3.229 us |           - |           - |           - |                   - |
//     SpanByteAndRead |   3.481 us | 0.0582 us | 0.0544 us |   3.466 us |           - |           - |           - |                   - |
//          UnsafeRead |   3.669 us | 0.0629 us | 0.0558 us |   3.656 us |           - |           - |           - |                   - |
//         UnsafeAsRef |   4.487 us | 0.0712 us | 0.0631 us |   4.478 us |           - |           - |           - |                   - |

//
// ===
//

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


//              Method |       Mean |     Error |    StdDev |     Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//-------------------- |-----------:|----------:|----------:|-----------:|------------:|------------:|------------:|--------------------:|
//         PInvokeAuto |  14.465 ms | 0.5230 ms | 1.5257 ms |  13.669 ms |           - |           - |           - |                   - |
//       PInvokeManual | 186.545 ms | 2.8366 ms | 2.3687 ms | 185.495 ms |  13333.3333 |           - |           - |          56000000 B |
// PInvokeSingleFields |  45.706 ms | 0.9057 ms | 2.2217 ms |  45.227 ms |           - |           - |           - |                   - |
//      SpanByteIntPtr |  10.745 ms | 0.1879 ms | 0.1569 ms |  10.786 ms |           - |           - |           - |                   - |
//            SpanByte |   3.552 ms | 0.1343 ms | 0.3677 ms |   3.427 ms |           - |           - |           - |                   - |
//          SpanStruct |   3.365 ms | 0.0669 ms | 0.1564 ms |   3.301 ms |           - |           - |           - |                   - |
//    SpanStructAndRef |   4.054 ms | 0.2821 ms | 0.8274 ms |   3.646 ms |           - |           - |           - |                   - |
//     SpanByteAndCast |   4.198 ms | 0.1917 ms | 0.5531 ms |   4.137 ms |           - |           - |           - |                   - |
//     SpanByteAndRead |   3.531 ms | 0.0606 ms | 0.0473 ms |   3.537 ms |           - |           - |           - |                   - |
//          UnsafeRead |   3.751 ms | 0.0767 ms | 0.1125 ms |   3.727 ms |           - |           - |           - |                   - |
//         UnsafeAsRef |   4.594 ms | 0.1103 ms | 0.1083 ms |   4.557 ms |           - |           - |           - |                   - |


namespace InteropConsole
{
    [MemoryDiagnoser]
    public class TestNative : IDisposable
    {
        //private const int Loop = 1_000;
        //private const int Loop = 1_000_000;

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
