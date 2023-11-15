using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception that is compound itself by multiple perceptions.
    /// </summary>
    public abstract class CompoundPerception : Perception
    {
        /// <summary>
        /// The list of subperceptions.
        /// </summary>
        public List<Perception> Perceptions;

        /// <summary>
        /// Create a new compound perception.
        /// </summary>
        public CompoundPerception()
        {
            Perceptions = new List<Perception>();
        }

        /// <summary>
        /// Create a new compound perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public CompoundPerception(List<Perception> perceptions)
        {
            Perceptions = perceptions;
        }

        /// <summary>
        /// Create a new compound perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public CompoundPerception(params Perception[] perceptions)
        {
            Perceptions = perceptions.ToList();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Initialize all the sub perceptions.
        /// </summary>
        public override void Initialize()
        {
            Perceptions.ForEach(p => p.Initialize());
        }

        /// <summary>
        /// <inheritdoc/>
        /// Reset all the sub perceptions.
        /// </summary>
        public override void Reset()
        {
            Perceptions.ForEach(p => p.Reset());
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses all the subp perceptions.
        /// </summary>
        public override void Pause()
        {
            Perceptions.ForEach(p => p.Reset());
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses all the subp perceptions.
        /// </summary>
        public override void Unpause()
        {
            Perceptions.ForEach(p => p.Reset());
        }

        /// <summary>
        /// Passes the execution context to the sub perceptions.
        /// </summary>
        /// <param name="context"><inheritdoc/></param>
        public override void SetExecutionContext(ExecutionContext context)
        {
            Perceptions.ForEach(p => p.SetExecutionContext(context));
        }

        /// <summary>
        /// <inheritdoc/>
        /// Copies the subperceptions one by one.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override object Clone()
        {
            CompoundPerception perception = (CompoundPerception)base.Clone();
            perception.Perceptions = Perceptions.Select(p => (Perception)p.Clone()).ToList();
            return perception;
        }
    }
}
