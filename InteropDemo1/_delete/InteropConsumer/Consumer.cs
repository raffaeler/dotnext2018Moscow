using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using InteropCommon;

namespace InteropConsumer
{
    public class Consumer : IConsumer
    {
        private IProducer _producer;
        public int _discard = 10;
        public int _counter;

        public Consumer(StrategyKind strategy)
        {
            Strategy = strategy;
        }

        public StrategyKind Strategy { get; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }

        public void Consume(IProducer producer)
        {
            if (_producer != null) Unsubscribe();

            Min = TimeSpan.MaxValue;
            Max = TimeSpan.Zero;
            _counter = 0;
            _producer = producer;
            Subscribe();
        }

        private void Subscribe()
        {
            switch (Strategy)
            {
                case StrategyKind.Managed:
                    _producer.OnDataReady += OnDataReadyManaged;
                    break;
                case StrategyKind.Unsafe:
                    _producer.OnDataReady += OnDataReadyUnsafe;
                    break;
                case StrategyKind.Span:
                    _producer.OnDataReady += OnDataReadyWithSpan;
                    break;
            }
        }

        private void Unsubscribe()
        {
            switch (Strategy)
            {
                case StrategyKind.Managed:
                    _producer.OnDataReady -= OnDataReadyUnsafe;
                    break;
                case StrategyKind.Unsafe:
                    _producer.OnDataReady -= OnDataReadyManaged;
                    break;
                case StrategyKind.Span:
                    _producer.OnDataReady -= OnDataReadyWithSpan;
                    break;
            }
        }

        private void OnDataReadyManaged(int length)
        {
            var ptr = _producer.Shared;

            var size = length / 8;
            var data = new Int64[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Marshal.ReadInt64(ptr);
                ptr += 8;
            }

            var timespan = TimeSpan.FromTicks(data[0]);
            //UpdateMinMax(timespan);
        }

        private void OnDataReadyManaged2(int length)
        {
            var ptr = _producer.Shared;
            var size = length / 8;
            //Buffer buffer = new Buffer();
            //buffer.Data = new long[size];
            //Int64[] data = new Int64[size];
            //Marshal.PtrToStructure<Buffer>(ptr, buffer);

            var data = new Int64[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Marshal.ReadInt64(ptr);
                Marshal.WriteInt64(ptr, data[i] + 1);
                ptr += 8;
            }

            var timespan = TimeSpan.FromTicks(data[0]);
            //UpdateMinMax(timespan);
        }

        private void OnDataReadyUnsafe(int length)
        {
            var blob = Marshal.AllocCoTaskMem(length);
            //CopyMemory(blob, _producer.Shared, length);
            Int64 ticks;
            unsafe
            {
                //Buffer.MemoryCopy(_producer.Shared.ToPointer(), blob.ToPointer(), length, length);
                //var ptr = (Int64*)blob.ToPointer();
                var ptr = (Int64*)_producer.Shared.ToPointer();
                ticks = *ptr;

                for (int i = 0; i < length; i++)
                {
                    ptr = (Int64*)_producer.Shared.ToPointer();
                    *ptr = *ptr + 1;
                    ptr++;
                }
            }

            var timespan = TimeSpan.FromTicks(ticks);
            //UpdateMinMax(timespan);
        }

        private void OnDataReadyWithSpan(int length)
        {
            Int64 ticks;
            Span<long> span;
            unsafe
            {
                span = new Span<Int64>(_producer.Shared.ToPointer(), _producer.WindowSize);
            }

            ticks = span[0];

            for (int i = 0; i < length; i++)
            {
                span[i]++;
            }

            var timespan = TimeSpan.FromTicks(ticks);
            //UpdateMinMax(timespan);
        }

        //private void UpdateMinMax(TimeSpan timeSpan)
        //{
        //    //if (timeSpan == TimeSpan.Zero) return;
        //    if (++_counter <= _discard) return;

        //    Int64 newMin = 0;
        //    Int64 newMax = 0;
        //    var min = Math.Min(timeSpan.Ticks, Min.Ticks);
        //    var max = Math.Max(timeSpan.Ticks, Max.Ticks);
        //    if (min < Min.Ticks)
        //    {
        //        newMin = min;
        //        Min = new TimeSpan(min);
        //    }

        //    if (max > Max.Ticks)
        //    {
        //        newMax = max;
        //        Max = new TimeSpan(max);
        //    }
        //}

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "RtlCopyMemory")]
        private extern static void CopyMemory(IntPtr destination, IntPtr source, int length);

        //[StructLayout(LayoutKind.Sequential)]
        //private class Buffer
        //{
        //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I8)]
        //    public Int64[] Data;
        //}

    }
}
