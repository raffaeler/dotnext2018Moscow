using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

//[assembly: System.Runtime.CompilerServices.ReferenceAssembly]

namespace PipeHelpers.Contracts
{
    public interface IInput
    {
        void Attach(PipeReader reader);
        Task Start();
    }
}
