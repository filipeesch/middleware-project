using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class MultiplyValueFilter : IPipelineFilter<int, int>
    {
        private readonly int multiplyBy;

        public MultiplyValueFilter(int multiplyBy)
        {
            this.multiplyBy = multiplyBy;
        }

        public Task Invoke(int input, Func<int, Task> next)
        {
            return next(input * this.multiplyBy);
        }
    }

    public class ByteArrayToStreamFilter : IPipelineFilter<byte[], Stream>
    {
        public async Task Invoke(byte[] input, Func<Stream, Task> next)
        {
            using (var stream = new MemoryStream(input))
            {
                await next(stream);
            }
        }
    }

    public class GzipCompressFilter : IPipelineFilter<Stream, Stream>
    {
        public async Task Invoke(Stream input, Func<Stream, Task> next)
        {
            using (var gzip = new GZipStream(input, CompressionMode.Compress))
            {
                await next(gzip);
            }
        }
    }

    public class LogElapsedExecutionTimeFilter<T> : IPipelineFilter<T, T>
    {
        public async Task Invoke(T input, Func<T, Task> next)
        {
            var sw = Stopwatch.StartNew();

            await next(input);

            sw.Stop();

            Console.WriteLine("Elapsed: {0}", sw.ElapsedMilliseconds);
        }
    }
}
