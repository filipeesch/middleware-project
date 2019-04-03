using System;

namespace KafkaTests
{
    public interface IPipelineFilter
    {
        Type InputType { get; }

        Type OutputType { get; }
    }
}