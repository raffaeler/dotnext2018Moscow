using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class ManyToOne : IInput, IOutput
    {
        private Task _startTask;
        public ManyToOne()
        {
            Pipe = new Pipe();
            Readers = new List<PipeReader>();
        }

        private Pipe Pipe { get; set; }
        private IList<PipeReader> Readers { get; set; }
        private IInput Next { get; set; }


        public void Connect(IInput input)
        {
            Next = input;
            Next.Attach(Pipe.Reader);
        }

        public void Attach(PipeReader reader)
        {
            this.Readers.Add(reader);
        }

        public Task Start()
        {
            if (_startTask != null) return _startTask;

            var nextTask = Next.Start();
            var copyTask = PipeUtilities.MultiReadAsync(Readers, Pipe.Writer,
                CanTransform, Transform).AsTask();

            _startTask = Task.WhenAll(nextTask, copyTask);
            return _startTask;
        }

        private bool CanTransform(TransformSource source)
        {
            if (source.IsCompleted || source.Memory.Length > 1000) return true;

            return false;
        }

        // This is definitely a poor implementation
        // The Pipeline API does not help much in this case
        // but there are more efficient (and more complex) ways to do it
        private Task<TransformResult> Transform(IList<TransformSource> sources)
        {
            var limit = (int)Math.Min(1000, sources.Min(s => s.Memory.Length));

            var inBuffers = new List<ReadOnlyMemory<byte>>();
            var frees = new List<byte[]>();
            foreach (var source in sources)
            {
                var buffer = ArrayPool<byte>.Shared.Rent((int)source.Memory.Length);
                source.Memory.CopyTo(buffer);
                source.Consumed = source.Memory.GetPosition(limit);
                inBuffers.Add(buffer);
                frees.Add(buffer);
            }

            var outbuffer = sources.First().Alloc(limit);
            TransformInternal(limit, inBuffers, outbuffer);

            foreach (var buffer in frees)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            var result = new TransformResult()
            {
                Result = outbuffer,
                Written = limit,
            };

            return Task.FromResult(result);
        }

        private void TransformInternal(int length, List<ReadOnlyMemory<byte>> inBuffers, Memory<byte> outbuffer)
        {
            for (int index = 0; index < length; index++)
            {
                outbuffer.Span[index] = (byte)inBuffers.Sum(s => s.Span[index]);
            }
        }
    }
}
