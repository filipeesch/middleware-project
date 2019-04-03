using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class PipelineFilter<TInput, TOutput> : IPipelineFilter
    {
        private readonly PipelineHandler<TInput, TOutput> handler;

        public PipelineFilter(PipelineHandler<TInput, TOutput> handler)
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
}
