using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core.Actions
{
    /// <summary>
    /// Action that runs a few actions in the same frame until one of them returns the specified Status value 
    /// or all the actions finish its execution.
    /// </summary>
    public class ParallelAction : CompoundAction
    {
        public bool finishOnSuccess;
        public bool finishOnFailure;

        List<Status> m_ExecutionResults;

        public ParallelAction() : base()
        {
            SubActions = new List<Action>();
            m_ExecutionResults = new List<Status>();
        }

        public ParallelAction(bool finishOnSuccess, bool finishOnFailure, List<Action> subActions) : base(subActions)
        {
            this.finishOnSuccess = finishOnSuccess;
            this.finishOnFailure = finishOnFailure;
            m_ExecutionResults = new List<Status>(subActions.Count);
        }

        public ParallelAction(bool finishOnSuccess, bool finishOnFailure, params Action[] subActions) : base(subActions)
        {
            this.finishOnSuccess = finishOnSuccess;
            this.finishOnFailure = finishOnFailure;
            m_ExecutionResults = new List<Status>(subActions.Length);
        }

        public override void Start()
        {
            m_ExecutionResults.Clear();

            foreach (Action action in SubActions)
            {
                action.Start();
                m_ExecutionResults.Add(Status.Running);
            }
        }

        public override Status Update()
        {
            if (SubActions.Count == 0) return Status.Failure;

            int currentActionId = 0;

            Status returnedStatus = Status.Running;
            Status currentActionStatus = Status.Running;
            bool anyActionRunning = false;

            while (currentActionId < SubActions.Count && returnedStatus == Status.Running)
            {
                if (m_ExecutionResults[currentActionId] == Status.Running)
                {
                    currentActionStatus = SubActions[currentActionId].Update();
                    m_ExecutionResults[currentActionId] = currentActionStatus;

                    if (currentActionStatus == Status.Running) anyActionRunning |= true;

                    if (finishOnSuccess && currentActionStatus == Status.Success || finishOnFailure && currentActionStatus == Status.Failure)
                    {
                        returnedStatus = currentActionStatus;
                    }
                }
                currentActionId++;
            }

            if(!anyActionRunning && returnedStatus == Status.Running)
            {
                return currentActionStatus;
            }
            else
            {
                return returnedStatus;
            }
        }

        public override void Stop()
        {
            m_ExecutionResults.Clear();
            SubActions.ForEach(a => a.Stop());
        }
    }
}
