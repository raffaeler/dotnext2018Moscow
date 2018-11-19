using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// These extensions are not as efficient as they could be
// the problem is that netstandard still does not include
// the extensions for ReadAsync and WriteAsync accepting Memory<T>

namespace PipeHelpers.Extensions
{
    /// <summary>
    /// Extension methods to copy the content between Streams and Pipelines
    /// </summary>
    public static class StreamPipeAdapter
    {
        private const int DefaultCopyBufferSize = 81920;

        /// <summary>
        /// Writes all the content of the Stream in the PipeWriter
        /// </summary>
        public static ValueTask CopyToAsync(this Stream stream, PipeWriter writer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return CopyToAsync(stream, writer, DefaultCopyBufferSize, cancellationToken);
        }

        /// <summary>
        /// Writes all the content of the Stream in the PipeWriter
        /// </summary>
        public static async ValueTask CopyToAsync(this Stream stream, PipeWriter writer, int bufferSize,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (bufferSize <= 0) throw new ArgumentException("buffer size cannot be negative", nameof(bufferSize));

            try
            {
                var buffer = new byte[bufferSize];
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, bufferSize, cancellationToken)
                        .ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var mem = new ReadOnlyMemory<byte>(buffer, 0, bytesRead);
                    var writeResult = await writer.WriteAsync(mem, cancellationToken);
                    // Advance must be used only if you don't use WriteAsync (which already calls Advance)
                    //writer.Advance(bytesRead);
                    var flush = await writer.FlushAsync(cancellationToken);
                    if(flush.IsCanceled || flush.IsCompleted)
                    {
                        break;
                    }
                }

                writer.Complete();
            }
            catch(Exception err)
            {
                writer.Complete(err);
            }
        }

        /// <summary>
        /// Writes all the content of the PipeReader in the Stream
        /// </summary>
        public static async Task CopyToAsync(this PipeReader reader, Stream target,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            try
            {
                while (true)
                {
                    //Console.WriteLine("start reading");
                    var read = await reader.ReadAsync(cancellationToken);
                    //Console.WriteLine("read ok");
                    var buffer = read.Buffer;

                    if (buffer.IsEmpty && read.IsCompleted)
                    {
                        break;
                    }

                    foreach (var memoryChunk in buffer)
                    {
                        await target.WriteAsync(memoryChunk.ToArray(), 0, memoryChunk.Length, cancellationToken);
                    }

                    reader.AdvanceTo(buffer.End);
                }

                reader.Complete();
                //Console.WriteLine("read Complete");
            }
            catch (Exception err)
            {
                reader.Complete(err);
            }
        }
    }
}
