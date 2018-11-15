``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17763.55 (1809/October2018Update/Redstone5)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.500-preview-009335
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|              Method |      Mean |    Error |   StdDev |
|-------------------- |----------:|---------:|---------:|
| TestProducerManaged | 300.87 ms | 3.445 ms | 3.223 ms |
|  TestProducerUnsafe |  71.03 ms | 1.411 ms | 1.320 ms |
|    TestProducerSpan |  71.48 ms | 1.367 ms | 1.404 ms |
