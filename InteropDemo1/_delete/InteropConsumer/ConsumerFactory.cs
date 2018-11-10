using System;
using System.Collections.Generic;
using System.Text;
using InteropCommon;

namespace InteropConsumer
{
    public class ConsumerFactory : IConsumerFactory
    {
        public Func<StrategyKind, IConsumer> Create => s => new Consumer(s);
    }
}
