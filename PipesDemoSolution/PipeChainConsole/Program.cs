using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PipeHelpers.Blocks;

namespace PipeChainConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var p = new Program();

            await p.Demo01_Copy();
            await p.Demo02_Nop();
            await p.Demo03_Log();
            await p.Demo04_OneToMany();
            await p.Demo05_ManyToOne();

            Console.WriteLine("== end ==");
            Console.ReadKey();
        }


        private async Task Demo01_Copy()
        {
            Console.WriteLine(nameof(Demo01_Copy));
            var testfile = "test.jpg";
            var source = GetSourcePath(testfile);
            var target = GetTargetFilename(source);

            var fileSource = new FileSource(source);
            var fileWriter = new FileWriter(target);

            fileSource.Connect(fileWriter);

            await fileSource.Start();
            AssertFilesAreEqual(source, target);
        }

        private async Task Demo02_Nop()
        {
            Console.WriteLine(nameof(Demo02_Nop));
            var testfile = "test.jpg";
            var source = GetSourcePath(testfile);
            var target = GetTargetFilename(source);

            var fileSource = new FileSource(source);
            var nop = new Nop();
            var fileWriter = new FileWriter(target);

            fileSource.Connect(nop);
            nop.Connect(fileWriter);

            await fileSource.Start();
            AssertFilesAreEqual(source, target);
        }

        private async Task Demo03_Log()
        {
            Console.WriteLine(nameof(Demo03_Log));
            var testfile = "test.jpg";
            var source = GetSourcePath(testfile);
            var target = GetTargetFilename(source);
            var log = GetTargetFilename(source, "_log");

            var fileSource = new FileSource(source);
            var logger = new Logger(log);
            var fileWriter = new FileWriter(target);

            fileSource.Connect(logger);
            logger.Connect(fileWriter);

            await fileSource.Start();
            AssertFilesAreEqual(source, target);
            AssertFilesAreEqual(source, log);
        }

        private async Task Demo04_OneToMany()
        {
            Console.WriteLine(nameof(Demo04_OneToMany));
            var testfile = "test.jpg";
            var source = GetSourcePath(testfile);
            var target1 = GetTargetFilename(source, "-target1");
            var target2 = GetTargetFilename(source, "-target2");

            var fileSource = new FileSource(source);
            var oneToMany = new OneToMany();
            var fileWriter1 = new FileWriter(target1);
            var fileWriter2 = new FileWriter(target2);

            fileSource.Connect(oneToMany);
            oneToMany.Connect(fileWriter1, fileWriter2);

            await fileSource.Start();
            AssertFilesAreEqual(source, target1);
            AssertFilesAreEqual(source, target2);
        }

        private async Task Demo05_ManyToOne()
        {
            Console.WriteLine(nameof(Demo05_ManyToOne));
            var length = 1200;
            System.IO.File.WriteAllBytes("ones.bin", GetArray(length, 1));
            System.IO.File.WriteAllBytes("twos.bin", GetArray(length, 2));

            var multiStart = new MultiStart();
            var fileSource1 = new FileSource("ones.bin");
            var fileSource2 = new FileSource("twos.bin");
            var manytoOne = new ManyToOne();
            var fileWriter = new FileWriter("threes.bin");

            multiStart.Connect(fileSource1, fileSource2);
            fileSource1.Connect(manytoOne);
            fileSource2.Connect(manytoOne);
            manytoOne.Connect(fileWriter);

            await multiStart.Start();
        }

        private byte[] GetArray(int length, byte content)
        {
            var blob = new byte[length];
            Array.Fill(blob, content);
            return blob;
        }

        private string GetSourcePath(string name)
        {
            var folderFullPath = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location),
                    @"..\..\"));

            string filename = Path.Combine(folderFullPath, name);
            if (File.Exists(filename))
            {
                return filename;
            }

            filename = Directory.GetFiles(folderFullPath, name, SearchOption.AllDirectories).FirstOrDefault();
            return filename;
        }

        private string GetTargetFilename(string filename, string suffix = "-target")
        {
            var target = Path.Combine(Path.GetDirectoryName(filename),
                $"{Path.GetFileNameWithoutExtension(filename) + suffix + Path.GetExtension(filename)}");

            return target;
        }

        private void AssertFilesAreEqual(string source, string target)
        {
            try
            {
                var b1 = File.ReadAllBytes(source);
                var b2 = File.ReadAllBytes(target);
                Debug.Assert(b1.SequenceEqual(b2), $"Files are different:\r\n{source}\r\n{target}");
            }
            catch (Exception err)
            {
                Debug.Fail(err.ToString());
            }
        }
    }
}
