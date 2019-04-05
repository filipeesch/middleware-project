﻿namespace KafkaTests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new WorkflowBuilder<int>();

            var workflow = builder
                .Use<int>(async (input, next) =>
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
                .Use(() => new LogElapsedExecutionTimeStep<int>())
                .Use<int>((input, next) => next(input * 2))
                .Use<int>((input, next) => next(input * 2))
                .Build();

            await workflow.Execute(10);

            string command;

            while ("exit" != (command = Console.ReadLine()))
            {
                if (!int.TryParse(command, out var number))
                    continue;

                var sw = Stopwatch.StartNew();

                var result = await workflow.Execute(number);

                sw.Stop();

                Console.WriteLine("Result: {0}", result);
                Console.WriteLine("total elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }
    }
}
