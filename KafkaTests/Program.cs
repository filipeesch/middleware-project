namespace KafkaTests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new PipelineBuilder<byte[]>();

            var pipeline = builder
                .Pipe<byte[]>(async (input, next) =>
                {
                    try
                    {
                        await next(input);
                    }
                    catch (Exception e)
                    {
                        //Log the exception
                        Console.WriteLine(e);
                    }
                })
                .Pipe(() => new LogElapsedExecutionTimeFilter<byte[]>())
                .Pipe(() => new ByteArrayToStreamFilter())
                .Pipe(() => new GzipCompressFilter())
                .Pipe<string>(async (input, next) => Console.WriteLine(input))
                .Build();

            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            await pipeline.Execute(data);

            string command;

            while ("exit" != (command = Console.ReadLine()))
            {
                if (!int.TryParse(command, out var number))
                    continue;

                var sw = Stopwatch.StartNew();

                await pipeline.Execute(number);

                sw.Stop();

                Console.WriteLine("elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }
    }
}
