using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

//     Method |    Loop |         Mean |       Error |      StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
//----------- |-------- |-------------:|------------:|------------:|------------:|------------:|------------:|--------------------:|
// StringTrim |    1000 |     23.25 us |   0.4561 us |   0.4684 us |     13.3362 |           - |           - |             56000 B |
//   SpanTrim |    1000 |     16.44 us |   0.3164 us |   0.4001 us |           - |           - |           - |                   - |
// StringTrim | 1000000 | 23,274.34 us | 448.1299 us | 613.4053 us |  13343.7500 |           - |           - |          56000000 B |
//   SpanTrim | 1000000 | 16,083.88 us | 226.4003 us | 200.6979 us |           - |           - |           - |                   - |

namespace InteropConsole
{
    [MemoryDiagnoser]
    public class TestSpanString
    {
        [Params(1_000, 1_000_000)]
        public int Loop { get; set; }

        public string SampleText { get; } = "  Hello, world    ";

        [Benchmark]
        public void StringTrim()
        {
            for(int i=0; i<Loop; i++)
            {
                string trimmed = SampleText.Trim();
            }
        }

        [Benchmark]
        public void SpanTrim()
        {
            ReadOnlySpan<char> sampleSpan = SampleText;
            for (int i = 0; i < Loop; i++)
            {
                ReadOnlySpan<char> trimmed = sampleSpan.Trim();
            }
        }

    }

    public static class SpanExtensions
    {
        public static Span<char> Trim(this Span<char> source)
        {
            if (source.IsEmpty)
                return source;

            int start = 0, end = source.Length - 1;
            char startChar = source[start], endChar = source[end];

            while ((start < end) && (startChar == ' ' || endChar == ' '))
            {
                if (startChar == ' ') start++;
                if (endChar == ' ') end--;
                startChar = source[start];
                endChar = source[end];
            }

            return source.Slice(start, end - start + 1);
        }
    }



}
