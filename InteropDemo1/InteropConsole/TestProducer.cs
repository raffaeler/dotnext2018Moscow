using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;


// TotalSize = 1024 * 1024 * 1024;
//              Method |      Mean |    Error |   StdDev |
//-------------------- |----------:|---------:|---------:|
// TestProducerManaged | 300.87 ms | 3.445 ms | 3.223 ms |
//  TestProducerUnsafe |  71.03 ms | 1.411 ms | 1.320 ms |
//    TestProducerSpan |  71.48 ms | 1.367 ms | 1.404 ms |

namespace InteropConsole
{
    //[MemoryDiagnoser]

    public class TestProducer : IDisposable
    {
        public const int TotalSize = 1024 * 1024 * 1024;
        private int Elements = TotalSize / 8;

        public IntPtr Shared { get; }

        private unsafe Int64* pu;
        private unsafe Int64* ps;

        public TestProducer()
        {
            Shared = Marshal.AllocCoTaskMem(TotalSize);
            //pm = Shared;
            unsafe { pu = (Int64*)Shared.ToPointer(); }
            unsafe { ps = (Int64*)Shared.ToPointer(); }
        }


        public void Dispose()
        {
            Marshal.FreeCoTaskMem(Shared);
        }

        [Benchmark]
        public void TestProducerManaged()
        {
            var pm = Shared;
            for (int i = 0; i < Elements; i++)
            {
                Marshal.WriteInt64(pm, 5);
                pm += 8;
            }
        }

        [Benchmark]
        public void TestProducerUnsafe()
        {
            unsafe
            {
                var p = pu;
                for (int i = 0; i < Elements; i++)
                {
                    *p = 9;
                    p++;
                }
            }
        }

        [Benchmark]
        public void TestProducerSpan()
        {
            unsafe
            {
                var span = new Span<Int64>(ps, Elements);
                span.Fill(100);
            }
        }

    }
}
