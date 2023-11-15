using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core.Actions
{
    public abstract class CompoundAction : Action
    {
        public List<Action> SubActions;

        protected CompoundAction()
        {
            SubActions = new List<Action>();
        }

        protected CompoundAction(List<Action> subActions)
        {
            SubActions = subActions;
        }

        protected CompoundAction(params Action[] subActions)
        {
            SubActions = subActions.ToList();
        }

        public override object Clone()
        {
            var clone = (SerialAction)MemberwiseClone();
            clone.SubActions = SubActions.Select(a => (Action)a.Clone()).ToList();
            return clone;
        }

        public override void SetExecutionContext(ExecutionContext context)
        {
            foreach (Action action in SubActions)
            {
                action.SetExecutionContext(context);
            }
        }
    }
}
