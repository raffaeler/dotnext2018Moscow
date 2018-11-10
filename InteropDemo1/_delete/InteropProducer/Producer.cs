using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using InteropCommon;

namespace InteropProducer
{
    public class Producer : IDisposable, IProducer
    {
        private Thread _worker;
        private TimeSpan _period;
        private ManualResetEventSlim _quit;

        public Producer(int id, StrategyKind strategy, int timesPerSec = 50, int windowSize = 1024, int totalSize = 1024 * 1024)
        {
            Strategy = strategy;
            TotalSize = totalSize;
            WindowSize = windowSize;
            TimesPerSec = timesPerSec;
            Shared = Marshal.AllocCoTaskMem(TotalSize);

            _period = TimeSpan.FromSeconds(1) / TimesPerSec;

            _quit = new ManualResetEventSlim(false);

            _worker = new Thread(Worker);
            _worker.Start(id);
        }

        public int TotalSize { get; }
        public int WindowSize { get; }
        public int TimesPerSec { get; }
        public StrategyKind Strategy { get; }
        public IntPtr Shared { get; }

        public event Action<int> OnDataReady;

        public void Dispose()
        {
            _quit.Set();
            _worker.Join();
            Marshal.FreeCoTaskMem(Shared);
            Debug.WriteLine("Thread closed and memory freed");
        }

        private void Worker(object obj)
        {
            if (!(obj is int id)) id = 0;
            var sw = new Stopwatch();
            sw.Start();

            Int64 previousTicks = 0;

            while (true)
            {
                if (_quit.IsSet) break;

                var before = sw.Elapsed;

                switch (Strategy)
                {
                    case StrategyKind.Unsafe:
                        ProduceWindowUnsafe(previousTicks);
                        break;

                    case StrategyKind.Managed:
                        ProduceWindowManaged(previousTicks);
                        break;

                    case StrategyKind.Span:
                        ProduceWindowWithSpan(previousTicks);
                        break;
                }

                var writeTime = sw.Elapsed - before;

                var sleepTime = _period - writeTime;
                if (sleepTime > TimeSpan.Zero)
                {
                    Thread.Sleep(_period - writeTime);
                }

                previousTicks = writeTime.Ticks;
            }
        }

        public void ProduceWindowManaged(Int64 previousTicks)
        {
            for (int i = 0; i < WindowSize; i += 8)
            {
                Marshal.WriteInt64(Shared, i, previousTicks);
            }

            OnDataReady?.Invoke(WindowSize);
        }

        public unsafe void ProduceWindowUnsafe(Int64 previousTicks)
        {
            var p = (Int64*)Shared.ToPointer();

            for (int i = 0; i < WindowSize; i++)
            {
                *p++ = previousTicks;
            }

            OnDataReady?.Invoke(WindowSize);
        }

        public unsafe void ProduceWindowWithSpan(Int64 previousTicks)
        {
            var span = new Span<Int64>(Shared.ToPointer(), WindowSize);

            span.Fill(previousTicks);

            OnDataReady?.Invoke(WindowSize);
        }


    }
}
