namespace KafkaTests
{
    using System;
    using System.Collections.Generic;

    public class WorkflowBuilderNode<TMainInput, TInput>
    {
        private readonly List<WorkflowStepInfo> steps;

        public WorkflowBuilderNode(List<WorkflowStepInfo> filters)
        {
            this.steps = filters;
        }

        public WorkflowBuilderNode<TMainInput, TOutput> Use<TOutput>(WorkflowHandler<TInput, TOutput> handler)
        {
            this.steps.Add(new WorkflowStepInfo(
                () => new HandlerWorkflowStep<TInput, TOutput>(handler),
                typeof(TInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TMainInput, TOutput>(this.steps);
        }

        public WorkflowBuilderNode<TMainInput, TOutput> Use<TOutput>(Func<IWorkflowStep<TInput, TOutput>> factory)
        {
            this.steps.Add(new WorkflowStepInfo(
                factory,
                typeof(TInput),
                typeof(TOutput)));

            return new WorkflowBuilderNode<TMainInput, TOutput>(this.steps);
        }

        public Workflow<TMainInput> Build()
        {
            return new Workflow<TMainInput>(this.steps);
        }
    }
}
