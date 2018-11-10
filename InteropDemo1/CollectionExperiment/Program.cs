using System;
using System.Linq;
using BenchmarkDotNet.Running;

namespace CollectionExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestsOnIntegers>();
        }
    }
}
