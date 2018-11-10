using System;
using System.Collections.Generic;
using System.Text;

namespace InteropCommon
{
    public interface IConsumerFactory
    {
        Func<StrategyKind, IConsumer> Create { get; }
    }
}
