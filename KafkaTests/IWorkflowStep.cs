using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public interface IWorkflowStep
    {
    }

    public interface IWorkflowStep<in TInput, out TOutput> : IWorkflowStep
    {
        Task Invoke(TInput input, Func<TOutput, Task> next);
    }
}
