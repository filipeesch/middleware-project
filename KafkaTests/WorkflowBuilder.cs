namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilder<TWorkflowInput>
    {
        public WorkflowBuilderNode<TWorkflowInput, TOutput> Use<TOutput>(WorkflowHandler<TWorkflowInput, TOutput> handler)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TWorkflowInput, TWorkflowInput>(steps);
            return node.Use(handler);
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput> Use<TOutput>(Func<IWorkflowStep<TWorkflowInput, TOutput>> factory)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TWorkflowInput, TWorkflowInput>(steps);
            return node.Use(factory);
        }
    }
}
