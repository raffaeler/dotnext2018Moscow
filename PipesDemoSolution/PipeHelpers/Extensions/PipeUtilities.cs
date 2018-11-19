using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeHelpers.Extensions
{
    public static class PipeUtilities
    {
        public static async ValueTask ContinuousReadAsync(PipeReader reader, PipeWriter writer,
            Func<TransformSource, bool> canTransform,
            Func<TransformSource, Task<TransformResult>> transform)
        {
            var source = new TransformSource(writer)
            {
                Reader = reader,
                Memory = ReadOnlySequence<byte>.Empty,
            };

            try
            {
                while (true)
                {
                    var read = await reader.ReadAsync();
                    var buffer = read.Buffer;
                    if (buffer.IsEmpty && read.IsCompleted)
                    {
                        break;
                    }

                    SequencePosition consumed;
                    //SequencePosition examined = buffer.End;
                    source.Memory = buffer;
                    if (canTransform(source))
                    {
                        var transformResult = await transform(source);
                        writer.Advance(transformResult.Result.Length);
                        //var writeResult = await writer.WriteAsync(transformResult.Result);
                        ////if (writeResult.IsCompleted)
                        ////{
                        ////    writer.Complete();
                        ////    break;
                        ////}

                        var flushResult = await writer.FlushAsync();
                        if (flushResult.IsCanceled) break;

                        consumed = source.Consumed;// buffer.GetPosition(transformResult.Consumed);
                    }
                    else
                    {
                        consumed = buffer.GetPosition(0);
                    }

                    //foreach (var memoryChunk in buffer)
                    //{
                    //    // process memoryChunk.Span
                    //    // do we have enough data to process this data?
                    //}

                    reader.AdvanceTo(consumed);//, examined);
                }

                reader.Complete();
                writer.Complete();
            }
            catch (Exception err)
            {
                reader.Complete(err);
            }
        }

        //static int written = 0;
        public static async ValueTask Short(PipeReader Reader, PipeWriter writer)
        {
            while (true)
            {
                var read = await Reader.ReadAsync();
                var buffer = read.Buffer;
                if (buffer.IsEmpty && read.IsCompleted)
                {
                    break;
                }

                FlushResult writeResult = new FlushResult(false, false);
                foreach(var segment in buffer)
                {
                    writeResult = await writer.WriteAsync(segment);
                    if (writeResult.IsCompleted) break;
                }

                if (writeResult.IsCompleted) break;
/*
                SequencePosition pos = buffer.Start;
                if (!buffer.TryGet(ref pos, out ReadOnlyMemory<byte> mem, true))
                {
                    break;
                }

                var writeResult = await writer.WriteAsync(mem);
                if (writeResult.IsCompleted) break;
*/
                Reader.AdvanceTo(buffer.End);
            }

            Reader.Complete();
            writer.Complete();
        }

        public static async ValueTask Broadcast(PipeReader reader, params PipeWriter[] writers)
        {
            await Broadcast(reader, (IEnumerable<PipeWriter>)writers);
        }

        public static async ValueTask Broadcast(PipeReader reader, IEnumerable<PipeWriter> writers)
        {
            while (true)
            {
                var read = await reader.ReadAsync();
                var buffer = read.Buffer;
                if (buffer.IsEmpty && read.IsCompleted)
                {
                    break;
                }

                //SequencePosition pos = buffer.Start;
                //if (!buffer.TryGet(ref pos, out ReadOnlyMemory<byte> mem, true))
                //{
                //    break;
                //}


                foreach (var segment in buffer)
                {
                    bool isOneWriterAlive = false;
                    foreach (var writer in writers)
                    {
                        var writeResult = await writer.WriteAsync(segment);
                        if (writeResult.IsCompleted) continue;

                        isOneWriterAlive = true;
                    }

                    if (!isOneWriterAlive)
                        break;
                }

                /*
                var blob = buffer.ToArray();
                var mem = new ReadOnlyMemory<byte>(blob);

                bool isOneWriterAlive = false;
                foreach (var writer in writers)
                {
                    var writeResult = await writer.WriteAsync(mem);
                    if (writeResult.IsCompleted) continue;

                    //var flushResult = await writer.FlushAsync();
                    //if (flushResult.IsCompleted) continue;
                    isOneWriterAlive = true;
                }

                if (!isOneWriterAlive)
                    break;
                */
                reader.AdvanceTo(buffer.End);
            }

            reader.Complete();
            foreach (var writer in writers)
            {
                writer.Complete();
            }
        }

        public static async ValueTask MultiReadAsync(IEnumerable<PipeReader> Readers, PipeWriter writer,
            Func<TransformSource, bool> canTransform,
            Func<IList<TransformSource>, Task<TransformResult>> transform)
        {
            try
            {
                while (true)
                {
                    var tasks = Readers
                        .Select(r => ReadAsync(r, canTransform).AsTask())
                        .ToList();

                    await Task.WhenAll(tasks);

                    var transformSources = tasks
                        .Select(t => t.Result)
                        .ToList();

                    if (transformSources.All(t => t.IsCompleted && t.Memory.IsEmpty))
                    {
                        break;
                    }

                    // transform will have to set the 'Result' and 'Consumed'
                    var transformResult = await transform(transformSources);

                    var writeResult = await writer.WriteAsync(transformResult.Result.Slice(0, transformResult.Written));
                    //if (writeResult.IsCompleted)
                    //{
                    //    writer.Complete();
                    //    break;
                    //}

                    //var flushResult = await writer.FlushAsync();
                    //if (flushResult.IsCanceled) break;


                    // now advance the readers
                    foreach (var context in transformSources)
                    {
                        if (context.Memory.IsEmpty)
                        {
                            continue;
                        }

                        context.Reader.AdvanceTo(context.Consumed);//, context.Memory.End);
                    }
                }

                foreach (var reader in Readers)
                {
                    reader.Complete();
                }

                writer.Complete();
            }
            catch (Exception err)
            {
                foreach (var reader in Readers)
                {
                    reader.Complete(err);   // TODO: complete with error only the exploded one
                }
            }
        }

        private static async ValueTask<TransformSource> ReadAsync(PipeReader reader,
            Func<TransformSource, bool> canTransform)
        {
            var source = new TransformSource()
            {
                Reader = reader,
                Memory = ReadOnlySequence<byte>.Empty,
            };

            try
            {
                while (true)
                {
                    var read = await reader.ReadAsync();
                    var buffer = read.Buffer;
                    source.IsCompleted = read.IsCompleted;

                    if (buffer.IsEmpty && read.IsCompleted)
                    {
                        return source;
                    }

                    SequencePosition consumed;
                    //SequencePosition examined = buffer.End;
                    source.Memory = buffer;

                    if (canTransform(source))
                    {
                        // will need to be advanced and/or completed
                        return source;
                    }
                    else
                    {
                        consumed = buffer.GetPosition(0);
                    }

                    reader.AdvanceTo(consumed);//, examined);
                }
            }
            catch (Exception err)
            {
                reader.Complete(err);
                return source;
            }
        }

    }
}