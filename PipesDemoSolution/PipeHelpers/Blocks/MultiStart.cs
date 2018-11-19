using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using PipeHelpers.Contracts;

namespace PipeHelpers.Blocks
{
    public class MultiStart : IOutput
    {
        public MultiStart()
        {
            Nexts = new List<IOutput>();
        }

        private List<IOutput> Nexts { get; set; }


        public void Connect(params IOutput[] inputs)
        {
            Connect((IEnumerable<IOutput>)inputs);
        }

        public void Connect(IEnumerable<IOutput> inputs)
        {
            Nexts.AddRange(inputs);
        }

        public Task Start()
        {
            var tasks = new List<Task>();
            foreach (var next in Nexts)
            {
                tasks.Add(next.Start());
            }

            return Task.WhenAll(tasks);
        }

    }
}
