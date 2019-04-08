namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilder<TWorkflowInput, TContext> where TContext : IWorkflowContext, new()
    {
        public WorkflowBuilderNode<TWorkflowInput, TOutput, TContext> Use<TOutput>(WorkflowHandler<TContext, TWorkflowInput, TOutput> handler)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TWorkflowInput, TWorkflowInput, TContext>(steps);
            return node.Use(handler);
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput, TContext> Use<TOutput>(Func<IWorkflowStep<TContext, TWorkflowInput, TOutput>> factory)
        {
            var steps = new List<WorkflowStepInfo>();

            var node = new WorkflowBuilderNode<TWorkflowInput, TWorkflowInput, TContext>(steps);
            return node.Use(factory);
        }
    }
}
