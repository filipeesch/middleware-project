using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class Pipeline<TInitialInput>
    {
        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Pipeline<TInitialInput>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<PipelineFilterInfo> filters;

        private readonly Dictionary<PipelineFilterInfo, Delegate> compilationCache =
            new Dictionary<PipelineFilterInfo, Delegate>();

        public Pipeline(List<PipelineFilterInfo> filters)
        {
            this.filters = filters;
        }

        public Task Execute(TInitialInput input)
        {
            return this.ExecuteStep(this.filters.GetEnumerator(), input);
        }

        private Task ExecuteStep(List<PipelineFilterInfo>.Enumerator info, object input)
        {
            if (!info.MoveNext())
                return Task.CompletedTask;

            var handler = this.GetCompiledHandler(info);

            var filter = info.Current.Create();

            return (Task)info.Current.InvokeMethod.Invoke(filter, new[] { input, handler });
        }

        private Delegate GetCompiledHandler(IEnumerator<PipelineFilterInfo> info)
        {
            if (this.compilationCache.TryGetValue(info.Current, out var cache))
                return cache;

            var compiled = CompileHandler(info);

            this.compilationCache.Add(info.Current, compiled);

            return compiled;
        }

        private Delegate CompileHandler(IEnumerator<PipelineFilterInfo> info)
        {
            var outParam = Expression.Parameter(info.Current.OutputType, "output");

            var lambda = Expression.Lambda(
                Expression.Call(
                    Expression.Constant(this),
                    ExecuteStepMethodInfo,
                    Expression.Constant(info),
                    Expression.Convert(
                        outParam,
                        typeof(object))),
                outParam);

            return lambda.Compile();
        }
    }
}
