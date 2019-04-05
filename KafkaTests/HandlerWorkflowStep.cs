namespace KafkaTests
{
    using System;
    using System.Threading.Tasks;

    public class HandlerWorkflowStep<TInput, TOutput> : IWorkflowStep<TInput, TOutput>
    {
        private readonly WorkflowHandler<TInput, TOutput> handler;

        public HandlerWorkflowStep(WorkflowHandler<TInput, TOutput> handler)
        {
            this.handler = handler;
        }

        public Task Invoke(TInput input, Func<TOutput, Task> next)
        {
            return this.handler(input, next);
        }
    }
}
