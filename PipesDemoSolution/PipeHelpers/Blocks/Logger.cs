using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class Logger : OneToOne
    {
        public Logger(string filename) : base()
        {
            Filename = filename;
        }

        public string Filename { get; private set; }

        public override void Connect(IInput input)
        {
            base.Connect(input);
        }

        public override void Attach(PipeReader reader)
        {
            base.Attach(reader);
            File.Delete(Filename);
        }

        public override Task Start()
        {
            return base.Start();
        }

        protected override bool CanTransform(TransformSource source)
        {
            return true;
        }

        protected override Task<TransformResult> Transform(TransformSource source)
        {
            int length = (int)source.Memory.Length;

            var buffer = source.Alloc(length);
            source.Consumed = source.Memory.GetPosition(length);

            var start = 0;
            foreach (var segment in source.Memory)
            {
                // write the log (append mode)
                using (var fs = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Seek(0, SeekOrigin.End);
                    fs.Write(segment.Span);
                }

                segment.CopyTo(buffer.Slice(start));
                start += segment.Length;
            }

            var transformResult = new TransformResult()
            {
                Result = buffer.Slice(0, length),
                Written = length,
            };

            return Task.FromResult(transformResult);
        }

        //protected override Task<TransformResult> Transform(TransformSource source)
        //{
        //    int length = (int)source.Memory.Length;

        //    var buffer = ArrayPool<byte>.Shared.Rent(length);
        //    source.Memory.CopyTo(buffer);
        //    source.Consumed = source.Memory.GetPosition(length);

        //    // write the log (append mode)
        //    using (var fs = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write))
        //    {
        //        fs.Seek(0, SeekOrigin.End);
        //        fs.Write(buffer, 0, length);
        //    }

        //    var transformResult = new TransformResult()
        //    {
        //        Result = buffer.AsMemory().Slice(0, length),
        //        Written = length,
        //    };

        //    return Task.FromResult(transformResult);
        //}

    }
}
