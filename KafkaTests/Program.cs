namespace KafkaTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var collection = new MiddlewareCollection();

            //pipeline.AddStep(new PipelineStep<int, int>((input, next) => next(input * 2)));

            collection.Add(new Middleware<int, int>((input, next) => input % 2 != 0 ? Task.CompletedTask : next(input)));

            collection.Add(new Middleware<int, int>(async (input, next) =>
            {
                try
                {
                    await next(input * 5);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }));

            collection.Add(new Middleware<int, string>((input, next) => next(Convert.ToString(input))));

            collection.Add(new Middleware<string, string>((input, next) => next(input)));

            string command;

            while ("exit" != (command = Console.ReadLine()))
            {
                if (!int.TryParse(command, out var number))
                    continue;

                var sw = Stopwatch.StartNew();

                await collection.Execute(number);

                sw.Stop();

                Console.WriteLine("elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }
    }

    public delegate Task MiddlewareHandler<in TInput, out TOutput>(TInput input, Func<TOutput, Task> next);


    public class Middleware<TInput, TOutput> : IPipelineStep
    {
        private readonly MiddlewareHandler<TInput, TOutput> handler;

        public Middleware(MiddlewareHandler<TInput, TOutput> handler)
        {
            this.handler = handler;
        }

        public async Task Invoke(TInput input, Func<TOutput, Task> next)
        {
            await this.handler(input, next);
        }

        public Type InputType => typeof(TInput);

        public Type OutputType => typeof(TOutput);
    }

    public interface IPipelineStep
    {
        Type InputType { get; }

        Type OutputType { get; }
    }

    public class MiddlewareCollection
    {
        private const string InvokeMethodName = nameof(Middleware<object, object>.Invoke);

        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(MiddlewareCollection).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<IPipelineStep> steps = new List<IPipelineStep>();

        private readonly Dictionary<IPipelineStep, (MethodInfo, Delegate)> compilationCache = new Dictionary<IPipelineStep, (MethodInfo, Delegate)>();

        public void Add<TInput, TOutput>(Middleware<TInput, TOutput> step)
        {
            this.steps.Add(step);
        }

        public async Task Execute<TInput>(TInput input)
        {
            await this.ExecuteStep(this.steps.GetEnumerator(), input);
        }

        private async Task ExecuteStep(List<IPipelineStep>.Enumerator stepEnum, object input)
        {
            if (!stepEnum.MoveNext())
                return;

            var (method, handler) = this.CompileStep(stepEnum);

            await (Task)method.Invoke(stepEnum.Current, new[] { input, handler });
        }

        private (MethodInfo, Delegate) CompileStep(IEnumerator<IPipelineStep> stepEnum)
        {
            var step = stepEnum.Current;

            if (this.compilationCache.TryGetValue(step, out var cache))
                return cache;

            var invokeStep = typeof(Middleware<,>)
                .MakeGenericType(step.InputType, step.OutputType)
                .GetMethod(InvokeMethodName);

            var outParam = Expression.Parameter(step.OutputType, "output");

            var lambda = Expression.Lambda(
                Expression.Call(
                    Expression.Constant(this),
                    ExecuteStepMethodInfo,
                    Expression.Constant(stepEnum),
                    Expression.Convert(
                        outParam,
                        typeof(object))),
                outParam);

            var compiled = lambda.Compile();

            this.compilationCache.Add(step, (invokeStep, compiled));

            return (invokeStep, compiled);
        }
    }
}
