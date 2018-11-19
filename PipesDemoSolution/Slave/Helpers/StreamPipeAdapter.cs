using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slave.Helpers
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
                while (true)
                {
                    var buffer = writer.GetMemory(bufferSize);
                    int bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writer.Advance(bytesRead);
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
                    var read = await reader.ReadAsync(cancellationToken);
                    var buffer = read.Buffer;

                    if (buffer.IsEmpty && read.IsCompleted)
                    {
                        break;
                    }

                    foreach (var memoryChunk in buffer)
                    {
                        await target.WriteAsync(memoryChunk, cancellationToken);
                    }

                    reader.AdvanceTo(buffer.End);
                }

                reader.Complete();
            }
            catch (Exception err)
            {
                reader.Complete(err);
            }
        }
    }
}
