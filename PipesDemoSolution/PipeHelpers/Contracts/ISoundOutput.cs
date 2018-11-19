using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace PipeHelpers.Contracts
{
    public interface IOutput
    {
        Task Start();
    }
}
