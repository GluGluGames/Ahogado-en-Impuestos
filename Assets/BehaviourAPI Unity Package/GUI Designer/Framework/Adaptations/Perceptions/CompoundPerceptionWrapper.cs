using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using Core.Perceptions;

    /// <summary>
    /// Adaptation wrapper class for use <see cref="CompoundPerception"/> in editor tools. 
    /// <para>! -- Don't use this class directly in code.</para>
    /// </summary>
    public class CompoundPerceptionWrapper : Perception, IBuildable
    {
        /// <summary>
        /// The wrapped compound perception.
        /// </summary>
        [SerializeReference] public CompoundPerception compoundPerception;

        /// <summary>
        /// The subperception serializable list.
        /// </summary>
        public List<PerceptionWrapper> subPerceptions = new List<PerceptionWrapper>();

        /// <summary>
        /// Parameterless constructor for reflection.
        /// </summary>
        public CompoundPerceptionWrapper()
        {
        }

        /// <summary>
        /// Create a new <see cref="CompoundPerceptionWrapper"></see> by a <see cref="CompoundPerception"/>. 
        /// </summary>
        /// <param name="compoundPerception">The compound perception.</param>
        public CompoundPerceptionWrapper(CompoundPerception compoundPerception)
        {
            this.compoundPerception = compoundPerception;
        }

        public override void Initialize() => compoundPerception.Initialize();

        public override bool Check() => compoundPerception.Check();

        public override void Reset() => compoundPerception.Reset();

        public override void Pause() => compoundPerception.Pause();

        public override void Unpause() => compoundPerception.Unpause();

        public override object Clone()
        {
            var copy = (CompoundPerceptionWrapper)base.Clone();
            copy.compoundPerception = (CompoundPerception)compoundPerception.Clone();
            copy.subPerceptions = subPerceptions.Select(p => (PerceptionWrapper)p.perception.Clone()).ToList();
            return copy;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Passes the context to <see cref="compoundPerception"/>.
        /// </summary>
        /// <param name="context">The execution contect.</param>
        public override void SetExecutionContext(ExecutionContext context)
        {
            compoundPerception.SetExecutionContext(context);
        }

        /// <summary>
        /// <inheritdoc/>
        /// Set <see cref="compoundPerception"/> subperceptions from <see cref="subPerceptions"/> list.
        /// </summary>
        /// <param name="data">The system data that contains the perception.</param>
        public void Build(BSBuildingInfo data)
        {
            foreach (var subPerception in subPerceptions)
                if (subPerception.perception is IBuildable buildable) buildable.Build(data);

            compoundPerception.Perceptions = subPerceptions.Select(p => p.perception).ToList();
        }

        public override string ToString()
        {
            var compoundType = compoundPerception.GetType();
            var logicCharacter = compoundType == typeof(AndPerception) ? " && " : compoundType == typeof(OrPerception) ? " || " : " - ";
            return "(" + string.Join(logicCharacter, subPerceptions.Select(sub => sub.perception?.ToString())) + ")";
        }

        public bool Validate(BSValidationInfo validationInfo) => subPerceptions.All(sub => (sub is IBuildable buildable) ? buildable.Validate(validationInfo) : true);
    }
}
