using System;

namespace KafkaTests
{
    public class PipelineBuilder<TInput>
    {
        private readonly Pipeline<TInput> pipeline = new Pipeline<TInput>();

        public PipelineBuilderNode<TInput, TOutput> Pipe<TOutput>(PipelineHandler<TInput, TOutput> handler)
        {
            this.pipeline.Pipe(handler);

            return new PipelineBuilderNode<TInput, TOutput>(this.pipeline);
        }

        public PipelineBuilderNode<TInput, TOutput> Pipe<TOutput>(Func<IPipelineFilter<TInput, TOutput>> factory)
        {
            this.pipeline.Pipe(factory);

            return new PipelineBuilderNode<TInput, TOutput>(this.pipeline);
        }
    }
}
