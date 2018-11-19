using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;
using PipeHelpers.Extensions;

namespace PipeHelpers.Blocks
{
    public class OneToMany : IInput, IOutput
    {
        public OneToMany()
        {
            Nexts = new List<(IInput, Pipe)>();
        }

        private IList<(IInput block, Pipe pipe)> Nexts { get; set; }
        private PipeReader Reader { get; set; }


        public void Connect(params IInput[] inputs)
        {
            Connect((IEnumerable<IInput>)inputs);
        }

        public void Connect(IEnumerable<IInput> inputs)
        {
            foreach (var next in inputs)
            {
                var pipe = new Pipe();
                Nexts.Add((next, pipe));
                next.Attach(pipe.Reader);
            }
        }

        public void Attach(PipeReader reader)
        {
            this.Reader = reader;
        }

        public Task Start()
        {
            var tasks = new List<Task>();
            foreach (var next in Nexts)
            {
                tasks.Add(next.block.Start());
            }

            tasks.Add(PipeUtilities.Broadcast(Reader, Nexts.Select(n => n.pipe.Writer)).AsTask());
            return Task.WhenAll(tasks);
        }

    }
}
