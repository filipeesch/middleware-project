using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public delegate Task WorkflowHandler<in TContext, in TInput, out TOutput>(TContext context, TInput input, Func<TOutput, Task> next);
}
