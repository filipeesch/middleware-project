using System;
using System.Reflection;

namespace KafkaTests
{
    public class PipelineFilterInfo
    {
        private readonly Func<IPipelineFilter> factory;

        public Type InputType { get; }

        public Type OutputType { get; }

        public PipelineFilterInfo(
            Func<IPipelineFilter> factory,
            Type inputType,
            Type outputType)
        {
            this.factory = factory;
            this.InputType = inputType;
            this.OutputType = outputType;

            this.InvokeMethod =
                typeof(IPipelineFilter<,>)
                    .MakeGenericType(inputType, outputType)
                    .GetMethod(nameof(IPipelineFilter<int, int>.Invoke));
        }

        public IPipelineFilter Create()
        {
            return this.factory();
        }

        public MethodInfo InvokeMethod { get; }
    }
}
