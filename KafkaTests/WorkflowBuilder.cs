namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilder<TMainInput>
    {
        public WorkflowBuilderNode<TMainInput, TOutput> Use<TOutput>(WorkflowHandler<TMainInput, TOutput> handler)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TMainInput, TMainInput>(steps);
            return node.Use(handler);
        }

        public WorkflowBuilderNode<TMainInput, TOutput> Use<TOutput>(Func<IWorkflowStep<TMainInput, TOutput>> factory)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TMainInput, TMainInput>(steps);
            return node.Use(factory);
        }
    }
}
