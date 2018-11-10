using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

// TotalSize = 1024 * 1024 * 1024
//      Method |      Mean |    Error |   StdDev |
//------------ |----------:|---------:|---------:|
// TestManaged | 475.58 ms | 4.925 ms | 4.607 ms |
//  TestUnsafe |  82.88 ms | 1.342 ms | 1.121 ms |
//    TestSpan |  89.39 ms | 1.715 ms | 1.605 ms |
// TestSpanRef |  89.84 ms | 1.702 ms | 1.821 ms |

namespace InteropConsole
{
    public class TestReadWriteStruct : IDisposable
    {
        public const int TotalSize = 1024 * 1024 * 1024;
        private int Elements = TotalSize / 8;
        public IntPtr Shared { get; }
        public unsafe long* ptr;


        public TestReadWriteStruct()
        {
            Shared = Marshal.AllocCoTaskMem(TotalSize);
            unsafe { ptr = (long*)Shared.ToPointer(); }
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(Shared);
        }


        [Benchmark]
        public void TestManaged()
        {
            var pm = Shared;
            for (int i = 0; i < Elements; i++)
            {
                var value = Marshal.ReadInt64(pm);
                Marshal.WriteInt64(pm, value + 1);
                pm += 8;
            }
        }

        [Benchmark]
        public void TestUnsafe()
        {
            unsafe
            {
                var pm = ptr;

                for (int i = 0; i < Elements; i++)
                {
                    var value = *pm;
                    *pm = value + 1;
                    pm++;
                }
            }
        }


        [Benchmark]
        public void TestSpan()
        {
            Span<long> span;
            unsafe
            {
                span = new Span<Int64>(ptr, Elements);
            }

            for (int i = 0; i < span.Length; i++)
            {
                span[i]++;
            }
        }

        [Benchmark]
        public void TestSpanRef()
        {
            Span<long> span;
            unsafe
            {
                span = new Span<Int64>(ptr, Elements);
            }

            for (int i = 0; i < span.Length; i++)
            {
                ref long value = ref span[i];
                value++;
            }
        }

    }
}
