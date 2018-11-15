using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace InteropConsole
{
    [MemoryDiagnoser]
    public class TestMemPressure
    {
        private int _size = 1024 *1024 + 1;

        [Params(1_000)]
        public int Loop { get; set; }

        [Benchmark]
        public void ArrayPoolPressure()
        {
            for (int i = 0; i < Loop; i++)
            {
                byte[] blob = ArrayPool<byte>.Shared.Rent(_size);
            }
        }
    }
}
