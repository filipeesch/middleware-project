namespace KafkaTests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new WorkflowBuilder<int, DefaultWorkflowContext<int>>();
            var bufferStep = new WorkflowBufferStep<int>(10);
            var bufferStep2 = new WorkflowBufferStep<int>(10);

            var workflow = builder
                .Use(() => bufferStep)
                .Use<int>(async (context, input, next) =>
                {
                    await Task.Delay(200);
                    Console.WriteLine("Workflow Middle: {0}", input);
                    await next(input);
                })
                .Use(() => bufferStep2)
                .Use<int>(async (context, input, next) =>
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Workflow Middle 1: {0}", input);
                    await next(input);
                })
                .Use<int>(async (context, input, next) => Console.WriteLine("Workflow Ended: {0}", input))
                .Build();

            string command;

            while ("exit" != (command = Console.ReadLine()))
            {
                if (!int.TryParse(command, out var number))
                    continue;


                for (int i = 0; i < number; i++)
                {
                    await workflow.Execute(i);
                    Console.WriteLine("Queued: {0}", i);
                }
            }
        }
    }
}
