using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class OneToOne : IInput, IOutput
    {
        public OneToOne()
        {
            Pipe = new Pipe();
        }

        protected Pipe Pipe { get; private set; }
        protected PipeReader Reader { get; private set; }
        protected IInput Next { get; private set; }

        public virtual void Connect(IInput input)
        {
            Next = input;
            Next.Attach(Pipe.Reader);
        }

        public virtual void Attach(PipeReader reader)
        {
            this.Reader = reader;
        }

        public virtual Task Start()
        {
            var nextTask = Next.Start();
            var copyTask = PipeUtilities.ContinuousReadAsync(Reader, Pipe.Writer, CanTransform, Transform).AsTask();
            return Task.WhenAll(nextTask, copyTask);
        }

        protected virtual bool CanTransform(TransformSource source)
        {
            return true;
        }


        protected virtual Task<TransformResult> Transform(TransformSource source)
        {
            int length = (int)source.Memory.Length;

            var memory = source.Alloc(length);
            source.Memory.CopyTo(memory.Span);
            source.Consumed = source.Memory.End;// GetPosition(length);

            var transformed = Transform(memory.Span);

            var transformResult = new TransformResult()
            {
                Result = transformed,
                Written = transformed.Length,
            };

            ////using (var fs = System.IO.File.Open("sample-aaa.wav", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            ////{
            ////    fs.Seek(0, System.IO.SeekOrigin.End);
            ////    fs.Write(buffer, 0, length);
            ////}

            ////var transformResult = new TransformResult()
            ////{
            ////    Result = buffer.AsMemory(),
            ////    Written = length,
            ////};


            return Task.FromResult(transformResult);
        }

        protected virtual Memory<byte> Transform(Span<byte> source)
        {
            var outbuffer = new byte[source.Length];
            source.CopyTo(outbuffer);
            return outbuffer;//new Memory<byte>(outbuffer);
        }
    }
}
