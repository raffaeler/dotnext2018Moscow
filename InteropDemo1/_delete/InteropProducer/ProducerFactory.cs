using System;
using System.Collections.Generic;
using System.Text;
using InteropCommon;

namespace InteropProducer
{
    public class ProducerFactory : IProducerFactory
    {
        public ProducerFactory(int timesPerSec = 50, int windowSize = 10 * 1024 * 1024, int totalSize = 100 * 1024 * 1024)
        {
            TotalSize = totalSize;
            WindowSize = windowSize;
            TimesPerSec = timesPerSec;
        }

        public int TotalSize { get; }
        public int WindowSize { get; }
        public int TimesPerSec { get; }

        public Func<int, StrategyKind, IProducer> Create => (id, strategy) => CreateProducer(id, strategy, TimesPerSec, WindowSize, TotalSize);

        public Producer CreateProducer(int id, StrategyKind strategy, int timesPerSec = 50, int windowSize = 1024, int totalSize = 1024 * 1024)
        {
            return new Producer(id, strategy, timesPerSec, windowSize, totalSize);
        }
    }
}
