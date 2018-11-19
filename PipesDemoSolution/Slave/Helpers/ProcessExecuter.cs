using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slave.Helpers
{
    internal class ProcessExecuter : IDisposable
    {
        private string _processFilename;
        private CancellationTokenSource _cts;
        private Process _process;

        public ProcessExecuter(string processFilename)
        {
            _processFilename = processFilename;
        }

        public void Dispose()
        {
            _cts.Cancel();
            if (!_process.WaitForExit(5000)) throw new Exception("Process is hung");
        }

        public Task ExecuteAsync(string[] arguments, PipeWriter target)
        {
            var argsString = string.Join(' ', arguments);
            return ExecuteAsync(argsString, target);
        }

        public async Task ExecuteAsync(string argsString, PipeWriter target)
        {
            _cts = new CancellationTokenSource();

            var processStartInfo = BuildOptions(argsString);
            _process = new Process();
            _process.EnableRaisingEvents = true;
            _process.StartInfo = processStartInfo;
            bool started = _process.Start();
            if (!started)
            {
                return;
            }

            var br = new BinaryReader(_process.StandardOutput.BaseStream);
            await br.BaseStream.CopyToAsync(target, _cts.Token);
            Console.WriteLine("Copy finished, disposing the process");
            _process.Dispose();
        }

        public async Task ExecuteAsync(string argsString, Func<Stream, ValueTask> stdoutProcessor)
        {
            _cts = new CancellationTokenSource();

            var processStartInfo = BuildOptions(argsString);
            _process = new Process();
            _process.EnableRaisingEvents = true;
            _process.StartInfo = processStartInfo;
            bool started = _process.Start();
            if (!started)
            {
                return;
            }

            var br = new BinaryReader(_process.StandardOutput.BaseStream);
            await stdoutProcessor(br.BaseStream);
            Console.WriteLine("Copy finished, disposing the process");
            _process.Dispose();
        }

        private ProcessStartInfo BuildOptions(string arguments)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = _processFilename;
            psi.Arguments = arguments;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.WorkingDirectory = Directory.GetCurrentDirectory();

            return psi;
        }
    }
}
