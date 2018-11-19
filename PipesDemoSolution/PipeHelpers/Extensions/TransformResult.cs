using System;
using System.Collections.Generic;
using System.Text;

namespace PipeHelpers.Extensions
{
    public class TransformResult
    {
        /// <summary>
        /// The output of the transformation
        /// </summary>
        public Memory<byte> Result { get; set; }

        /// <summary>
        /// The amound of bytes consumed from Memory
        /// </summary>
        public int Written { get; set; }
    }
}
