using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public interface IPipelineFilter
    {
    }

    public interface IPipelineFilter<in TInput, out TOutput> : IPipelineFilter
    {
        Task Invoke(TInput input, Func<TOutput, Task> next);
    }
}