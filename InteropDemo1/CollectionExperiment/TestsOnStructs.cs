using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Raf.Collections;

// LoopLength = 1_000
//           Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//----------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
//     TestXListAdd | 4.582 ms | 0.0911 ms | 0.1860 ms |    953.1250 |    929.6875 |    929.6875 |           8384857 B |
//      TestListAdd | 4.570 ms | 0.0898 ms | 0.1932 ms |    960.9375 |    929.6875 |    929.6875 |           8389089 B |
// TestXListIndexer | 1.035 ms | 0.0202 ms | 0.0354 ms |           - |           - |           - |                   - |
//  TestListIndexer | 1.665 ms | 0.0282 ms | 0.0264 ms |           - |           - |           - |                   - |
// TestXListForEach | 1.520 ms | 0.0303 ms | 0.0444 ms |           - |           - |           - |                   - |
//  TestListForEach | 2.079 ms | 0.0415 ms | 0.0737 ms |           - |           - |           - |                   - |

// LoopLength = 1_000_000
//           Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//----------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
//     TestXListAdd | 4.599 ms | 0.0918 ms | 0.1287 ms |    953.1250 |    929.6875 |    929.6875 |           8384857 B |
//      TestListAdd | 4.589 ms | 0.0907 ms | 0.1790 ms |    960.9375 |    929.6875 |    929.6875 |           8389089 B |
// TestXListIndexer | 1.030 ms | 0.0206 ms | 0.0366 ms |           - |           - |           - |                   - |
//  TestListIndexer | 1.685 ms | 0.0335 ms | 0.0522 ms |           - |           - |           - |                   - |
// TestXListForEach | 1.551 ms | 0.0305 ms | 0.0493 ms |           - |           - |           - |                   - |
//  TestListForEach | 2.085 ms | 0.0410 ms | 0.0718 ms |           - |           - |           - |                   - |

namespace CollectionExperiment
{
    [MemoryDiagnoser]
    public class TestsOnStructs
    {
        //private const int LoopLength = 1_000_000;
        private const int LoopLength = 1_000;

        private XList<MyStruct> _intTestXList;
        private List<MyStruct> _intTestList;
        private MyStruct _sample = new MyStruct(6789);

        public TestsOnStructs()
        {
            _intTestXList = new XList<MyStruct>();
            _intTestList = new List<MyStruct>();
            for (int i = 0; i < LoopLength; i++)
            {
                var s = new MyStruct(i);
                _intTestList.Add(s);
                _intTestXList.Add(s);
            }

        }

        [Benchmark]
        public void TestXListAdd()
        {
            var list = new XList<MyStruct>();
            for (int i = 0; i < LoopLength; i++)
            {
                list.Add(_sample);
            }
        }

        [Benchmark]
        public void TestListAdd()
        {
            var list = new List<MyStruct>();
            for (int i = 0; i < LoopLength; i++)
            {
                list.Add(_sample);
            }
        }

        [Benchmark]
        public void TestXListIndexer()
        {
            for (int i = 0; i < _intTestXList.Count; i++)
            {
                _intTestXList[i] = _sample;
            }
        }

        [Benchmark]
        public void TestListIndexer()
        {
            for (int i = 0; i < _intTestList.Count; i++)
            {
                _intTestList[i] = _sample;
            }
        }

        [Benchmark]
        public void TestXListForEach()
        {
            foreach (ref readonly var item in _intTestXList)
            {
                var i = item;
            }
        }

        [Benchmark]
        public void TestListForEach()
        {
            foreach (var item in _intTestList)
            {
                var i = item;
            }
        }

        private struct MyStruct
        {
            public MyStruct(int n)
            {
                Id = n;
                Flag = (byte)(n % 256);
                Unique = Guid.NewGuid();
                Date = DateTimeOffset.Now;
                Number = n / 256;
            }

            public int Id { get; set; }
            public byte Flag { get; set; }
            public Guid Unique { get; set; }
            public DateTimeOffset Date { get; set; }
            public decimal Number { get; set; }
        }

    }
}
