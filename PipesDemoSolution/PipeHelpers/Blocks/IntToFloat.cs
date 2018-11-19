using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class IntToFloat : OneToOne
    {
        private int b_min = int.MaxValue;
        private int b_max = int.MinValue;
        
        private float b2_min = float.MaxValue;
        private float b2_max = float.MinValue;

        public IntToFloat()
        {
        }

        protected override Task<TransformResult> Transform(TransformSource source)
        {
            var length = (int)source.Memory.Length;
            source.Consumed = source.Memory.End;

            var memory = source.Alloc(length * 4);
            var spanFloats = MemoryMarshal.Cast<byte, float>(memory.Span);
            
            // Instead of just copying the byte array to floats ...
            //Utilities.CopyToFloat(source.Memory, spanFloats, length);


            // ... we constrain the value of the float array to the interval [-1, +1)
            int offset = 0;
            foreach (var segment in source.Memory)
            {
                for (int i = 0; i < segment.Length; i++)
                {
                    if (offset >= length)  // target.Length
                        break;

                    var input = segment.Span;

                    //spanFloats[offset] = input[i];

                    b_min = Math.Min(b_min, input[i]);
                    b_max = Math.Max(b_max, input[i]);

                    // TODO: this version is handling only 8 bit per sample
                    //temp[offset] = input[i] / 32768f;
                    //temp[offset] = input[i] / 256f;
                    spanFloats[offset] = (input[i] - 128) / 128.0f;

                    b2_min = Math.Min(b2_min, spanFloats[offset]);
                    b2_max = Math.Max(b2_max, spanFloats[offset]);

                    offset++;
                }
            }

            // finally we return the byte[] representation of our floats
            var transformResult = new TransformResult()
            {
                Result = memory,
                Written = memory.Length,
            };

            return Task.FromResult(transformResult);
        }
    }
}
