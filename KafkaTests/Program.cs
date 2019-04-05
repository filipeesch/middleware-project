namespace KafkaTests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new PipelineBuilder<int>();

            var pipeline = builder
                .Pipe<int>(async (input, next) =>
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
                .Pipe<int>(() => new LogElapsedExecutionTimeFilter<int>())
                .Pipe<int>((input, next) => next(input * 2))
                .Pipe<string>(async (input, next) => Console.WriteLine(input))
                .Build();

            await pipeline.Execute(10);

            string command;

            while ("exit" != (command = Console.ReadLine()))
            {
                if (!int.TryParse(command, out var number))
                    continue;

                var sw = Stopwatch.StartNew();

                await pipeline.Execute(number);

                sw.Stop();

                Console.WriteLine("total elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }
    }
}
