using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class Nop : IInput, IOutput
    {
        public Nop()
        {
            Pipe = new Pipe();
        }

        private Pipe Pipe { get; set; }
        private PipeReader Reader { get; set; }
        private IInput Next { get; set; }


        public void Connect(IInput input)
        {
            Next = input;
            Next.Attach(Pipe.Reader);
        }

        public void Attach(PipeReader reader)
        {
            this.Reader = reader;
        }

        public Task Start()
        {
            var nextTask = Next.Start();
            var copyTask = PipeUtilities.Short(Reader, Pipe.Writer).AsTask();
            return Task.WhenAll(nextTask, copyTask);
        }

    }
}
