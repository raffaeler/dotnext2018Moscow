using System;
using System.Collections.Generic;
using System.Text;

namespace InteropCommon
{
    public interface IProducer : IDisposable
    {
        int TotalSize { get; }
        int WindowSize { get; }
        int TimesPerSec { get; }
        StrategyKind Strategy { get; }
        IntPtr Shared { get; }
        event Action<int> OnDataReady;
    }
}
