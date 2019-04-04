using System;

namespace KafkaTests
{
    public class PipelineBuilderNode<TMainInput, TInput>
    {
        private readonly Pipeline<TMainInput> pipeline;

        public PipelineBuilderNode(Pipeline<TMainInput> pipeline)
        {
            this.pipeline = pipeline;
        }

        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(PipelineHandler<TInput, TOutput> handler)
        {
            this.pipeline.Pipe(handler);

            return new PipelineBuilderNode<TMainInput, TOutput>(this.pipeline);
        }

        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(Func<IPipelineFilter<TInput, TOutput>> factory)
        {
            this.pipeline.Pipe(factory);

            return new PipelineBuilderNode<TMainInput, TOutput>(this.pipeline);
        }

        public Pipeline<TMainInput> Build()
        {
            return this.pipeline;
        }
    }
}
