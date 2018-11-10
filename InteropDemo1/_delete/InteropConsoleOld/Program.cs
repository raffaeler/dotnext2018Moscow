using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using InteropCommon;
using InteropConsumer;
using InteropProducer;

namespace InteropConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<TestProducer>();
            BenchmarkRunner.Run<TestReadWrite>();

            //new Program().Start();
            //Console.ReadKey();
        }

        private async void Start()
        {
            var duration = TimeSpan.FromSeconds(3);
            await StartTest(duration, 0, StrategyKind.Managed, StrategyKind.Managed);
            await StartTest(duration, 0, StrategyKind.Managed, StrategyKind.Unsafe);
            await StartTest(duration, 0, StrategyKind.Managed, StrategyKind.Span);

            await StartTest(duration, 0, StrategyKind.Unsafe, StrategyKind.Managed);
            await StartTest(duration, 0, StrategyKind.Unsafe, StrategyKind.Unsafe);
            await StartTest(duration, 0, StrategyKind.Unsafe, StrategyKind.Span);

            await StartTest(duration, 0, StrategyKind.Span, StrategyKind.Managed);
            await StartTest(duration, 0, StrategyKind.Span, StrategyKind.Unsafe);
            await StartTest(duration, 0, StrategyKind.Span, StrategyKind.Span);
        }

        private async Task StartTest(TimeSpan duration, int producerId, StrategyKind producerStrategy, StrategyKind consumerStrategy)
        {
            Console.Write($"Producer: {producerStrategy,-10} - Consumer: {consumerStrategy,-10} ");

            var producerFactory = new ProducerFactory();
            using (var producer = producerFactory.Create(producerId, producerStrategy))
            {
                var consumerFactory = new ConsumerFactory();
                var consumer = consumerFactory.Create(consumerStrategy);

                consumer.Consume(producer);

                await Task.Delay(duration);
                Console.WriteLine($"Min:{consumer.Min.TotalMilliseconds,6:0.00}ms - Max:{consumer.Max.TotalMilliseconds,6:0.00}ms");
            }
        }
    }
}
