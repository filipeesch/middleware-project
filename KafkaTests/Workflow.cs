namespace KafkaTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    public class Workflow<TInput, TOutput>
    {
        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Workflow<TInput, TOutput>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<WorkflowStepInfo> infos;

        private readonly Dictionary<WorkflowStepInfo, Delegate> compilationCache =
            new Dictionary<WorkflowStepInfo, Delegate>();

        private object output;

        public Workflow(List<WorkflowStepInfo> steps)
        {
            this.infos = steps;

            this.CompileHandlers();
        }

        private void CompileHandlers()
        {
            var info = this.infos.GetEnumerator();

            while (info.MoveNext())
            {
                this.compilationCache.Add(info.Current, this.CompileHandler(info));
            }
        }

        public async Task<TOutput> Execute(TInput input)
        {
            await this.ExecuteStep(this.infos.GetEnumerator(), input);

            return this.output == null ?
                default :
                (TOutput)this.output;
        }

        private Task ExecuteStep(List<WorkflowStepInfo>.Enumerator info, object input)
        {
            if (!info.MoveNext())
            {
                this.output = input;
                return Task.CompletedTask;
            }

            var handler = this.compilationCache[info.Current];

            return info.Current.InvokeStep(input, handler);
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
