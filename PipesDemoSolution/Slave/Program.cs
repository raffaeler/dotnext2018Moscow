using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Slave.Helpers;

namespace Slave
{
    class Program
    {
        public static readonly string MasterExe = @"..\..\..\..\Master\bin\debug\master.exe";
        static void Main(string[] args)
        {
            var filename = Path.GetFullPath(MasterExe);
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Can't find master: {filename}");
                return;
            }

            var targetFile = "result.jpg";

            ProcessStdout(filename, targetFile).Wait();
            Console.ReadKey();
        }

        private static async Task ProcessStdout(string sourceFile, string targetFile)
        {
            var pipe = new Pipe();

            var executer = new ProcessExecuter(sourceFile);

            using (var targetStream = File.Create(targetFile))
            {
                var readerTask = pipe.Reader.CopyToAsync(targetStream);
                var writerTask = executer.ExecuteAsync(string.Empty, s => s.CopyToAsync(pipe.Writer));

                await Task.WhenAll(readerTask, writerTask);
            }
        }
    }
}
