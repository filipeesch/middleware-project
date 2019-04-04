using System;
using System.Threading.Tasks;

namespace KafkaTests
{
    public class MultiplyValueFilter : IPipelineFilter<int, int>
    {
        private readonly int multiplyBy;

        public MultiplyValueFilter(int multiplyBy)
        {
            this.multiplyBy = multiplyBy;
        }

        public Task Invoke(int input, Func<int, Task> next)
        {
            return next(input * this.multiplyBy);
        }
    }
}
