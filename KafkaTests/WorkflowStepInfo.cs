using System;
using System.Reflection;

namespace KafkaTests
{
    using System.Threading.Tasks;

    public class WorkflowStepInfo
    {
        private readonly Func<IWorkflowStep> factory;
        private readonly MethodInfo invokeMethod;

        public Type InputType { get; }

        public Type OutputType { get; }

        public WorkflowStepInfo(
            Func<IWorkflowStep> factory,
            Type contextType,
            Type inputType,
            Type outputType)
        {
            this.factory = factory;
            this.InputType = inputType;
            this.OutputType = outputType;

            this.invokeMethod =
                typeof(IWorkflowStep<,,>)
                    .MakeGenericType(contextType, inputType, outputType)
                    .GetMethod(nameof(IWorkflowStep<int, int, int>.Invoke));
        }

        public async Task InvokeStep(object context, object input, object handler)
        {
            var step = this.factory();

            await (Task)this.invokeMethod.Invoke(step, new[] { context, input, handler });
        }
    }
}
