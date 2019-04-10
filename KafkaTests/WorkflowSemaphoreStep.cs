namespace KafkaTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class WorkflowSemaphoreStep<T> : IWorkflowStep<T, T>
    {
        private readonly SemaphoreSlim semaphore;

        public WorkflowSemaphoreStep(SemaphoreSlim semaphore)
        {
            this.semaphore = semaphore;
        }

        public async Task Invoke(IWorkflowContext context, T input, Func<T, Task> next)
        {
            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                await next(input).ConfigureAwait(false);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
