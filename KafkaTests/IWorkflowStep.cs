using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public interface IWorkflowStep
    {
    }

    public interface IWorkflowStep<in TContext, in TInput, out TOutput> : IWorkflowStep
    {
        Task Invoke(TContext context, TInput input, Func<TOutput, Task> next);
    }

    public interface IWorkflowStep<in TInput, out TOutput> : IWorkflowStep<IWorkflowContext, TInput, TOutput>
    {
    }
}
