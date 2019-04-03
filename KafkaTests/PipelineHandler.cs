using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public delegate Task PipelineHandler<in TInput, out TOutput>(TInput input, Func<TOutput, Task> next);
}
