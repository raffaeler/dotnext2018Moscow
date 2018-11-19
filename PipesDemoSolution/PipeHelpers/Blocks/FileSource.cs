using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class FileSource : IOutput
    {
        public FileSource(string filename)
        {
            this.Filename = filename;
            this.Pipe = new Pipe();
        }

        private Pipe Pipe { get; }
        private IInput Next { get; set; }
        public string Filename { get; }
        public CancellationToken CancellationToken { get; private set; } = default(CancellationToken);

        public void Connect(IInput input)
        {
            Next = input;
            Next.Attach(Pipe.Reader);
        }

        public async Task Start()
        {
            var nextTask = Next.Start();

            using (var fs = File.OpenRead(Filename))
            {
                var copyTask = fs.CopyToAsync(Pipe.Writer);

                await Task.WhenAll(nextTask, copyTask.AsTask());
            }
        }
    }
}
