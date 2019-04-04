using System;

namespace KafkaTests
{
    public class PipelineFilterInfo
    {
        private readonly Func<IPipelineFilter> factory;

        public Type InputType { get; }

        public Type OutputType { get; }

        public PipelineFilterInfo(Func<IPipelineFilter> factory, Type inputType, Type outputType)
        {
            this.factory = factory;
            this.InputType = inputType;
            this.OutputType = outputType;
        }

        public IPipelineFilter Create()
        {
            return this.factory();
        }
    }
}
