``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17763.55 (1809/October2018Update/Redstone5)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.500-preview-009335
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|              Method |       Mean |     Error |    StdDev |     Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------------- |-----------:|----------:|----------:|-----------:|------------:|------------:|------------:|--------------------:|
|         PInvokeAuto |  13.133 us | 0.2099 us | 0.1963 us |  13.071 us |           - |           - |           - |                   - |
|       PInvokeManual | 189.085 us | 3.6932 us | 4.3965 us | 187.135 us |     13.1836 |           - |           - |             56000 B |
| PInvokeSingleFields |  46.824 us | 0.9284 us | 2.6489 us |  46.807 us |           - |           - |           - |                   - |
|      SpanByteIntPtr |  12.013 us | 0.3914 us | 1.1540 us |  11.774 us |           - |           - |           - |                   - |
|            SpanByte |   2.892 us | 0.0550 us | 0.0487 us |   2.893 us |           - |           - |           - |                   - |
|          SpanStruct |   2.879 us | 0.0463 us | 0.0362 us |   2.870 us |           - |           - |           - |                   - |
|    SpanStructAndRef |   3.481 us | 0.1315 us | 0.3793 us |   3.296 us |           - |           - |           - |                   - |
|     SpanByteAndCast |   3.471 us | 0.1128 us | 0.3218 us |   3.320 us |           - |           - |           - |                   - |
|     SpanByteAndRead |   4.380 us | 0.2124 us | 0.6263 us |   4.278 us |           - |           - |           - |                   - |
|          UnsafeRead |   4.000 us | 0.2224 us | 0.6237 us |   3.698 us |           - |           - |           - |                   - |
|         UnsafeAsRef |   5.190 us | 0.2643 us | 0.7668 us |   4.884 us |           - |           - |           - |                   - |
