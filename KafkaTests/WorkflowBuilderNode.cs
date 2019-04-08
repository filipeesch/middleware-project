namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilderNode<TWorkflowInput, TStepInput, TContext> where TContext : IWorkflowContext, new()
    {
        private readonly List<WorkflowStepInfo> steps;

        public WorkflowBuilderNode(List<WorkflowStepInfo> filters)
        {
            this.steps = filters;
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput, TContext> Use<TOutput>(WorkflowHandler<TContext, TStepInput, TOutput> handler)
        {
            this.steps.Add(new WorkflowStepInfo(
                () => new HandlerWorkflowStep<TContext, TStepInput, TOutput>(handler),
                typeof(TContext),
                typeof(TStepInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TWorkflowInput, TOutput, TContext>(this.steps);
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput, TContext> Use<TOutput>(Func<IWorkflowStep<TContext, TStepInput, TOutput>> factory)
        {
            this.steps.Add(new WorkflowStepInfo(
                factory,
                typeof(TContext),
                typeof(TStepInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TWorkflowInput, TOutput, TContext>(this.steps);
        }

        public Workflow<TWorkflowInput, TContext> Build()
        {
            return new Workflow<TWorkflowInput, TContext>(this.steps);
        }
    }
}
