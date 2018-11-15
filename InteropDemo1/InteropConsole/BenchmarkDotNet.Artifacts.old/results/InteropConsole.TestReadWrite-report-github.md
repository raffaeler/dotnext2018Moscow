``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17763.55 (1809/October2018Update/Redstone5)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.500-preview-009335
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|      Method |      Mean |    Error |   StdDev |
|------------ |----------:|---------:|---------:|
| TestManaged | 475.58 ms | 4.925 ms | 4.607 ms |
|  TestUnsafe |  82.88 ms | 1.342 ms | 1.121 ms |
|    TestSpan |  89.39 ms | 1.715 ms | 1.605 ms |
| TestSpanRef |  89.84 ms | 1.702 ms | 1.821 ms |
