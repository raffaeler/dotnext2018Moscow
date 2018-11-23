using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//[MedianColumn, MinColumn, Q1Column]
//[Benchmark(Baseline =true)]
//[StatisticalTestColumn()]

namespace InteropConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var proc = Environment.Is64BitProcess ? "x64" : "x86";
            Console.WriteLine($"Process running at: {proc}");
            //BenchmarkRunner.Run<TestProducer>();
            //BenchmarkRunner.Run<TestReadWrite>();
            //BenchmarkRunner.Run<TestReadWriteStruct>();
            BenchmarkRunner.Run<TestNative>();
            //BenchmarkRunner.Run<TestMemPressure>();
            //BenchmarkRunner.Run<TestSpanString>();

            //var testNative = new TestNative();
            //testNative.PInvokeSingleFields();

            //TestInteropNative();

            //new Program().Start();
            //Console.ReadKey();
        }

        private static void TestInteropNative()
        {
            var native = new NativeInterop(@"assets\dsp_demo_sample.wav");

            native.Read(out IntPtr data, out int length);
            var wavheader1 = Marshal.PtrToStructure<WavHeader>(data);

            unsafe
            {
                var ptr = native.ReadUnsafe();

                var spanByte = new Span<byte>(ptr, sizeof(WavHeader));

                var spanWavHeader = new Span<WavHeader>(ptr, 1);
                ref var refSpanWavHeader = ref MemoryMarshal.GetReference<WavHeader>(spanWavHeader);

                var wavheaderx1 = MemoryMarshal.Cast<byte, WavHeader>(spanByte);

                var wavheader2 = MemoryMarshal.Read<WavHeader>(spanByte);
            }

            WavHeader wavHeader3 = native.ReadWavHeader();
            Debug.Assert(wavHeader3.NumChannels == 2);
        }
    }
}
