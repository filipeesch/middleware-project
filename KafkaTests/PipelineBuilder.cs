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
    }
}
