using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class Pipeline<TInitialInput>
    {
        private const string InvokeMethodName = nameof(GenericPipelineFilter<object, object>.Invoke);

        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Pipeline<TInitialInput>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<PipelineFilterInfo> filters = new List<PipelineFilterInfo>();

        private readonly Dictionary<PipelineFilterInfo, (MethodInfo, Delegate)> compilationCache =
            new Dictionary<PipelineFilterInfo, (MethodInfo, Delegate)>();

        public void Pipe<TInput, TOutput>(PipelineHandler<TInput, TOutput> handler)
        {
            this.filters.Add(new PipelineFilterInfo(
                () => new GenericPipelineFilter<TInput, TOutput>(handler),
                typeof(TInput),
                typeof(TOutput)));
        }

        public void Pipe<TInput, TOutput>(Func<IPipelineFilter<TInput, TOutput>> factory)
        {
            this.filters.Add(new PipelineFilterInfo(
                factory,
                typeof(TInput),
                typeof(TOutput)));
        }

        public Task Execute(TInitialInput input)
        {
            return this.ExecuteStep(this.filters.GetEnumerator(), input);
        }

        private Task ExecuteStep(List<PipelineFilterInfo>.Enumerator infoEnum, object input)
        {
            if (!infoEnum.MoveNext())
                return Task.CompletedTask;

            var (method, handler) = this.CompileStep(infoEnum);

            var filter = infoEnum.Current.Create();

            return (Task)method.Invoke(filter, new[] { input, handler });
        }

        private (MethodInfo, Delegate) CompileStep(IEnumerator<PipelineFilterInfo> infoEnum)
        {
            var info = infoEnum.Current;

            if (this.compilationCache.TryGetValue(info, out var cache))
                return cache;

            var invokeStep = typeof(IPipelineFilter<,>)
                .MakeGenericType(info.InputType, info.OutputType)
                .GetMethod(InvokeMethodName);

            var outParam = Expression.Parameter(info.OutputType, "output");

            var lambda = Expression.Lambda(
                Expression.Call(
                    Expression.Constant(this),
                    ExecuteStepMethodInfo,
                    Expression.Constant(infoEnum),
                    Expression.Convert(
                        outParam,
                        typeof(object))),
                outParam);

            var compiled = lambda.Compile();

            this.compilationCache.Add(info, (invokeStep, compiled));

            return (invokeStep, compiled);
        }
    }
}
