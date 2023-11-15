using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using BehaviourAPI.Core.Perceptions;
    using Core;
    using Core.Actions;

    /// <summary>
    /// Adaptation wrapper class for use <see cref="CompoundAction"/> in editor tools. 
    /// <para>! -- Don't use this class directly in code.</para>
    /// </summary>
    public class CompoundActionWrapper : Action, IBuildable
    {
        /// <summary>
        /// The wrapped compound action.
        /// </summary>
        [SerializeReference] public CompoundAction compoundAction;

        /// <summary>
        /// The subactiopn serializable list.
        /// </summary>
        public List<SubActionWrapper> subActions = new List<SubActionWrapper>();

        /// <summary>
        /// Parameterless constructor for reflection.
        /// </summary>
        public CompoundActionWrapper()
        {
        }

        /// <summary>
        /// Create a new <see cref="CompoundActionWrapper"></see> by a <see cref="CompoundAction"/>. 
        /// </summary>
        /// <param name="compoundPerception">The compound action.</param>
        public CompoundActionWrapper(CompoundAction compoundAction)
        {
            this.compoundAction = compoundAction;
        }

        public override void Start() => compoundAction.Start();

        public override Status Update() => compoundAction.Update();

        public override void Stop() => compoundAction.Stop();

        public override void Pause() => compoundAction.Pause();

        public override void Unpause() => compoundAction.Unpause();

        public override object Clone()
        {
            var copy = (CompoundActionWrapper)base.Clone();
            copy.compoundAction = (CompoundAction)compoundAction.Clone();
            copy.subActions = subActions.Select(p => new SubActionWrapper((Action)p.action.Clone())).ToList();
            return copy;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Passes the context to <see cref="compoundPerception"/>.
        /// </summary>
        /// <param name="context">The execution contect.</param>
        public override void SetExecutionContext(ExecutionContext context)
        {
            compoundAction.SetExecutionContext(context);
        }

        /// <summary>
        /// <inheritdoc/>
        /// Set <see cref="compoundAction"/> subperceptions from <see cref="subActions"/> list.
        /// </summary>
        /// <param name="data">The system data that contains the action.</param>
        public void Build(BSBuildingInfo data)
        {
            foreach(var subAction in subActions)
                if (subAction.action is IBuildable buildable) buildable.Build(data);

            compoundAction.SubActions = subActions.Select(p => p.action).ToList();
        }

        public override string ToString()
        {
            var compoundType = compoundAction.GetType();
            var logicCharacter = compoundType == typeof(SequenceAction) ? " >> " : compoundType == typeof(ParallelAction) ? " | " : " - ";
            return "(" + string.Join(logicCharacter, subActions.Select(sub => sub.action?.ToString())) + ")";
        }

        public bool Validate(BSValidationInfo validationInfo) => subActions.All(sub => (sub is IBuildable buildable) ? buildable.Validate(validationInfo) : true);
    }
}
