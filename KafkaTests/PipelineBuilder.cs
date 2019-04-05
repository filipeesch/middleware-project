using System;

namespace KafkaTests
{
    using System.Collections.Generic;

    public class PipelineBuilder<TMainInput>
    {
        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(PipelineHandler<TMainInput, TOutput> handler)
        {
            var filters = new List<PipelineFilterInfo>();

            var node = new PipelineBuilderNode<TMainInput, TMainInput>(filters);
            return node.Pipe(handler);
        }

        public PipelineBuilderNode<TMainInput, TOutput> Pipe<TOutput>(Func<IPipelineFilter<TMainInput, TOutput>> factory)
        {
            var filters = new List<PipelineFilterInfo>();

            var node = new PipelineBuilderNode<TMainInput, TMainInput>(filters);
            return node.Pipe(factory);
        }
    }
}
