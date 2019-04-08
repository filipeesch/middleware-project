namespace KafkaTests
{
    public interface IWorkflowContext<TOutput> : IWorkflowContext
    {
        TOutput Output { get; set; }
    }

    public interface IWorkflowContext
    {
    }
}
