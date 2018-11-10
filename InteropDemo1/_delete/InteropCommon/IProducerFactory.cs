using System;
using System.Collections.Generic;
using System.Text;

namespace InteropCommon
{
    public interface IProducerFactory
    {
        Func<int, StrategyKind, IProducer> Create { get; }
    }
}
