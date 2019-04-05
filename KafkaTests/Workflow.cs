using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class Workflow<TInitialInput>
    {
        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Workflow<TInitialInput>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<WorkflowStepInfo> steps;

        private readonly Dictionary<WorkflowStepInfo, Delegate> compilationCache =
            new Dictionary<WorkflowStepInfo, Delegate>();

        public Workflow(List<WorkflowStepInfo> filters)
        {
            this.steps = filters;
        }

        public Task Execute(TInitialInput input)
        {
            return this.ExecuteStep(this.steps.GetEnumerator(), input);
        }

        private Task ExecuteStep(List<WorkflowStepInfo>.Enumerator info, object input)
        {
            if (!info.MoveNext())
                return Task.CompletedTask;

            var handler = this.GetCompiledHandler(info);

            return info.Current.InvokeStep(input, handler);
        }

        private Delegate GetCompiledHandler(IEnumerator<WorkflowStepInfo> info)
        {
            if (this.compilationCache.TryGetValue(info.Current, out var cache))
                return cache;

            var compiled = this.CompileHandler(info);

            this.compilationCache.Add(info.Current, compiled);

            return compiled;
        }

        private Delegate CompileHandler(IEnumerator<WorkflowStepInfo> info)
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
