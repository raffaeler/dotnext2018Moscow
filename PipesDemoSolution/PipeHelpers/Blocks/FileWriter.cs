using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class FileWriter : IInput
    {
        public FileWriter(string filename)
        {
            this.Filename = filename;
        }

        public string Filename { get; }

        // the reader of the previous block
        private PipeReader Reader { get; set; }

        public void Attach(PipeReader reader)
        {
            this.Reader = reader;
        }

        public async Task Start()
        {
            using (var fs = File.OpenWrite(Filename))
            {
                await Reader.CopyToAsync(fs);
            }
        }
    }
}
