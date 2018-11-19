using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class FloatToInt : OneToOne
    {
        private float a_min = float.MaxValue;
        private float a_max = float.MinValue;

        private int a2_min = int.MaxValue;
        private int a2_max = int.MinValue;

        protected override Task<TransformResult> Transform(TransformSource source)
        {
            var length = (int)source.Memory.Length;
            source.Consumed = source.Memory.End;
            var output = source.Alloc(length / 4);
            var outSpan = output.Span;

            int offset = 0;
            float dataOffset = 1.4f;//(b2_max - b2_min) / 2;
            foreach (var segment in source.Memory)
            {
                var dataSource = MemoryMarshal.Cast<byte, float>(segment.Span);
                for (int i = 0; i < dataSource.Length; i++)
                {
                    if (offset >= length)  // target.Length
                        break;

                    var input = segment.Span;

                    a_min = Math.Min(a_min, dataSource[i]);
                    a_max = Math.Max(a_max, dataSource[i]);

                    //outSpan[offset] = (byte)((dataSource[i] - 0.5f) * 8192);
                    //outSpan[offset] = (byte)((dataSource[i]) * 150.0f);
                    //outSpan[offset] = (byte)((dataSource[i] + 1.0f) * 12/8.0f);
                    outSpan[offset] = (byte)((dataSource[i] + dataOffset) * (256.0f / (2 * dataOffset)));


                    a2_min = Math.Min(a2_min, outSpan[offset]);
                    a2_max = Math.Max(a2_max, outSpan[offset]);

                    offset++;
                }
            }


            // end
            var transformResult = new TransformResult()
            {
                Result = output,
                Written = output.Length,
            };

            return Task.FromResult(transformResult);
        }
    }
}
