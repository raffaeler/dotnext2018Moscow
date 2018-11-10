using System;
using System.Collections.Generic;
using System.Text;

namespace InteropCommon
{
    public interface IConsumer
    {
        void Consume(IProducer producer);

        StrategyKind Strategy { get; }
        TimeSpan Min { get; }
        TimeSpan Max { get; }
    }
}
