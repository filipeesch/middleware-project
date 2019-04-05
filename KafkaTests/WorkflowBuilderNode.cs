namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilderNode<TWorkflowInput, TStepInput>
    {
        private readonly List<WorkflowStepInfo> steps;

        public WorkflowBuilderNode(List<WorkflowStepInfo> filters)
        {
            this.steps = filters;
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput> Use<TOutput>(WorkflowHandler<TStepInput, TOutput> handler)
        {
            this.steps.Add(new WorkflowStepInfo(
                () => new HandlerWorkflowStep<TStepInput, TOutput>(handler),
                typeof(TStepInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TWorkflowInput, TOutput>(this.steps);
        }

        public WorkflowBuilderNode<TWorkflowInput, TOutput> Use<TOutput>(Func<IWorkflowStep<TStepInput, TOutput>> factory)
        {
            this.steps.Add(new WorkflowStepInfo(
                factory,
                typeof(TStepInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TWorkflowInput, TOutput>(this.steps);
        }

        public Workflow<TWorkflowInput, TStepInput> Build()
        {
            return new Workflow<TWorkflowInput, TStepInput>(this.steps);
        }
    }
}
