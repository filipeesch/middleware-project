using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class Pipeline<TInput>
    {
        private const string InvokeMethodName = nameof(PipelineFilter<object, object>.Invoke);

        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Pipeline<TInput>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<IPipelineFilter> filters = new List<IPipelineFilter>();

        private readonly Dictionary<IPipelineFilter, (MethodInfo, Delegate)> compilationCache = new Dictionary<IPipelineFilter, (MethodInfo, Delegate)>();

        public void Pipe<TInput, TOutput>(PipelineHandler<TInput, TOutput> handler)
        {
            this.filters.Add(new PipelineFilter<TInput, TOutput>(handler));
        }

        public async Task Execute(TInput input)
        {
            await this.ExecuteStep(this.filters.GetEnumerator(), input);
        }

        private async Task ExecuteStep(List<IPipelineFilter>.Enumerator stepEnum, object input)
        {
            if (!stepEnum.MoveNext())
                return;

            var (method, handler) = this.CompileStep(stepEnum);

            await (Task)method.Invoke(stepEnum.Current, new[] { input, handler });
        }

        private (MethodInfo, Delegate) CompileStep(IEnumerator<IPipelineFilter> stepEnum)
        {
            var step = stepEnum.Current;

            if (this.compilationCache.TryGetValue(step, out var cache))
                return cache;

            var invokeStep = typeof(PipelineFilter<,>)
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
