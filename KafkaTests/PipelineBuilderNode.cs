using System;

namespace KafkaTests
{
    using System.Collections.Generic;

    public class PipelineBuilderNode<TMainInput, TInput>
    {
        private readonly List<PipelineFilterInfo> filters;

        public PipelineBuilderNode(List<PipelineFilterInfo> filters)
        {
            this.filters = filters;
        }

        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(PipelineHandler<TInput, TOutput> handler)
        {
            this.filters.Add(new PipelineFilterInfo(
                () => new GenericPipelineFilter<TInput, TOutput>(handler),
                typeof(TInput),
                typeof(TOutput)));

            return new PipelineBuilderNode<TMainInput, TOutput>(this.filters);
        }

        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(Func<IPipelineFilter<TInput, TOutput>> factory)
        {
            this.filters.Add(new PipelineFilterInfo(
                factory,
                typeof(TInput),
                typeof(TOutput)));

            return new PipelineBuilderNode<TMainInput, TOutput>(this.filters);
        }

        public Pipeline<TMainInput> Build()
        {
            return new Pipeline<TMainInput>(this.filters);
        }
    }
}
