namespace KafkaTests
{
    public class DefaultWorkflowContext<TOutput> : IWorkflowContext<TOutput>
    {
        public TOutput Output { get; set; }
    }
}