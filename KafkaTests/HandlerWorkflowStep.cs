namespace KafkaTests
{
    using System;
    using System.Threading.Tasks;

    public class HandlerWorkflowStep<TContext, TInput, TOutput> : IWorkflowStep<TContext, TInput, TOutput>
    {
        private readonly WorkflowHandler<TContext, TInput, TOutput> handler;

        public HandlerWorkflowStep(WorkflowHandler<TContext, TInput, TOutput> handler)
        {
            this.handler = handler;
        }

        public Task Invoke(TContext context, TInput input, Func<TOutput, Task> next)
        {
            return this.handler(context, input, next);
        }
    }
}
