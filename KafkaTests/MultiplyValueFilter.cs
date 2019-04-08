namespace KafkaTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    public class MultiplyValueStep : IWorkflowStep<int, int>
    {
        private readonly int multiplyBy;

        public MultiplyValueStep(int multiplyBy)
        {
            this.multiplyBy = multiplyBy;
        }

        public Task Invoke(IWorkflowContext context, int input, Func<int, Task> next)
        {
            return next(input * this.multiplyBy);
        }
    }

    public class ByteArrayToStreamStep : IWorkflowStep<byte[], Stream>
    {
        public async Task Invoke(IWorkflowContext context, byte[] input, Func<Stream, Task> next)
        {
            using (var stream = new MemoryStream(input))
            {
                await next(stream);
            }
        }
    }

    public class GzipCompressStep : IWorkflowStep<Stream, Stream>
    {
        public async Task Invoke(IWorkflowContext context, Stream input, Func<Stream, Task> next)
        {
            using (var gzip = new GZipStream(input, CompressionMode.Compress))
            {
                await next(gzip);
            }
        }
    }

    public class LogElapsedExecutionTimeStep<T> : IWorkflowStep<T, T>
    {
        public async Task Invoke(IWorkflowContext context, T input, Func<T, Task> next)
        {
            var sw = Stopwatch.StartNew();

            await next(input);

            sw.Stop();

            Console.WriteLine("Elapsed: {0}", sw.ElapsedMilliseconds);
        }
    }
}
