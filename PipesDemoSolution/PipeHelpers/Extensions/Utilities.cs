using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PipeHelpers.Blocks
{
    public static class Utilities
    {
        /// <summary>
        /// Get the nearest next power of two of the given number
        /// </summary>
        public static int GetNextPowerOfTwo(int number)
        {
            number--;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            number++;
            return number;
        }

        /// <summary>
        /// Copy an array of byte into an array of float
        /// No casts occur
        /// </summary>
        public static void CopyToFloat(ReadOnlySequence<byte> source, Span<float> target, int samples)
        {
            int offset = 0;
            foreach (var segment in source)
            {
                for (int i = 0; i < segment.Length; i++)
                {
                    if (offset >= samples)  // target.Length
                        return;

                    target[offset] = segment.Span[i];
                    offset++;
                }
            }
        }

        /// <summary>
        /// Casts the input float array into a byte array, then copy it to the target byte array.
        /// This is used to preserve memory layout of float array but the call is expressed in terms of byte array
        /// </summary>
        public static void CopyCastFloatsToBytes(ReadOnlySpan<float> floats, Span<byte> target)
        {
            var casted = MemoryMarshal.Cast<float, byte>(floats);
            casted.CopyTo(target);
        }

        /// <summary>
        /// The source is made of float array. Since it is made of not contiguous segments, this method:
        /// - enumerates the segments
        /// - cast the memory segment to float
        /// - copy the resulting floats to the target array
        /// </summary>
        public static void CastAndCopyFloatToFloat(ReadOnlySequence<byte> source, Span<float> target, int samples)
        {
            int offset = 0;
            foreach (var segment in source)
            {
                // data in memory is already an array of floats
                // since we got bytes, we just have to "cast" from byte[] to float[]
                var dataSource = MemoryMarshal.Cast<byte, float>(segment.Span);
                for (int i = 0; i < dataSource.Length; i++)
                {
                    if (offset >= samples)
                        return;

                    target[offset] = dataSource[i];
                    offset++;
                }
            }
        }

        /// <summary>
        /// Copy the floats to bytes by truncating the value to the byte
        /// This is a lossy conversion which presumes the previous code already windowed
        /// the values to 0-255
        /// </summary>
        public static void CopyFloatsToBytes(ReadOnlySpan<float> floats, Span<byte> target, int samples)
        {
            for (int i = 0; i < samples; i++)
            {
                target[i] = (byte)floats[i];
            }
        }

    }
}
