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
                .Pipe<int>((input, next) => input % 2 != 0 ? Task.CompletedTask : next(input))
                .Pipe(() => new MultiplyValueFilter(5))
                .Pipe<int>(async (input, next) =>
                {
                    try
                    {
                        await next(input * 5);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                })
                .Pipe<string>((input, next) => next(Convert.ToString(input)))
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

                Console.WriteLine("elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }
    }
}
