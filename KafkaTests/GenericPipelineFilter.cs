using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class GenericPipelineFilter<TInput, TOutput> : IPipelineFilter<TInput, TOutput>
    {
        private readonly PipelineHandler<TInput, TOutput> handler;

        public GenericPipelineFilter(PipelineHandler<TInput, TOutput> handler)
        {
            this.handler = handler;
        }

        public Task Invoke(TInput input, Func<TOutput, Task> next)
        {
            return this.handler(input, next);
        }
    }
}
