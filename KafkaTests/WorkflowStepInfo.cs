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
            Type inputType,
            Type outputType)
        {
            this.factory = factory;
            this.InputType = inputType;
            this.OutputType = outputType;

            this.invokeMethod =
                typeof(IWorkflowStep<,>)
                    .MakeGenericType(inputType, outputType)
                    .GetMethod(nameof(IWorkflowStep<int, int>.Invoke));
        }

        public Task InvokeStep(object input, object handler)
        {
            var step = this.factory();

            return (Task)this.invokeMethod.Invoke(step, new[] { input, handler });
        }
    }
}
