using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Raf.Collections;

// LoopLength = 1_000
//           Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//----------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
//     TestXListAdd | 2.373 us | 0.0465 us | 0.0826 us |      0.9995 |           - |           - |              4200 B |
//      TestListAdd | 2.844 us | 0.0563 us | 0.0602 us |      2.0065 |           - |           - |              8432 B |
// TestXListIndexer | 1.061 us | 0.0212 us | 0.0336 us |           - |           - |           - |                   - |
//  TestListIndexer | 1.676 us | 0.0326 us | 0.0435 us |           - |           - |           - |                   - |
// TestXListForEach | 1.587 us | 0.0311 us | 0.0615 us |           - |           - |           - |                   - |
//  TestListForEach | 2.297 us | 0.0460 us | 0.1028 us |           - |           - |           - |                   - |
//

// LoopLength = 1_000_000
//           Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//----------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
//     TestXListAdd | 4.597 ms | 0.0909 ms | 0.1773 ms |    953.1250 |    929.6875 |    929.6875 |           8384857 B |
//      TestListAdd | 4.299 ms | 0.0826 ms | 0.0984 ms |    960.9375 |    929.6875 |    929.6875 |           8389089 B |
// TestXListIndexer | 1.036 ms | 0.0203 ms | 0.0360 ms |           - |           - |           - |                   - |
//  TestListIndexer | 1.696 ms | 0.0334 ms | 0.0468 ms |           - |           - |           - |                   - |
// TestXListForEach | 1.541 ms | 0.0306 ms | 0.0495 ms |           - |           - |           - |                   - |
//  TestListForEach | 2.091 ms | 0.0415 ms | 0.0727 ms |           - |           - |           - |                   - |
//

namespace CollectionExperiment
{
    [MemoryDiagnoser]
    public class TestsOnIntegers
    {
        private const int LoopLength = 1_000_000;
        //private const int LoopLength = 1_000;

        private XList<int> _intTestXList;
        private List<int> _intTestList;

        public TestsOnIntegers()
        {
            _intTestXList = new XList<int>();
            _intTestList = new List<int>();
            for (int i = 0; i < LoopLength; i++)
            {
                _intTestList.Add(i);
                _intTestXList.Add(i);
            }

        }

        [Benchmark]
        public void TestXListAdd()
        {
            var list = new XList<int>();
            for (int i = 0; i < LoopLength; i++)
            {
                list.Add(i);
            }
        }

        [Benchmark]
        public void TestListAdd()
        {
            var list = new List<int>();
            for (int i = 0; i < LoopLength; i++)
            {
                list.Add(i);
            }
        }

        [Benchmark]
        public void TestXListIndexer()
        {
            for (int i = 0; i < _intTestXList.Count; i++)
            {
                _intTestXList[i] = i;
            }
        }

        [Benchmark]
        public void TestListIndexer()
        {
            for (int i = 0; i < _intTestList.Count; i++)
            {
                _intTestList[i] = i;
            }
        }

        [Benchmark]
        public void TestXListForEach()
        {
            foreach (ref readonly var item in _intTestXList)
            {
                int i = item;
            }
        }

        [Benchmark]
        public void TestListForEach()
        {
            foreach (var item in _intTestList)
            {
                int i = item;
            }
        }

    }
}
