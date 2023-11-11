using System.Collections.Generic;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Compound perception that returns true when any of the subperception return true.
    /// </summary>
    public class OrPerception : CompoundPerception
    {
        /// <summary>
        /// Create a new or perception.
        /// </summary>
        public OrPerception() : base() { }

        /// <summary>
        /// Create a new or perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public OrPerception(List<Perception> perceptions) : base(perceptions) { }

        /// <summary>
        /// Create a new or perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public OrPerception(params Perception[] perceptions) : base(perceptions) { }

        /// <summary>
        /// Check all the sub perceptions and return true if any of them returned true.
        /// Returns false if the sub perception list is empty.
        /// </summary>
        /// <returns>true if any of the sub perceptions returned true and the sub perception list is not empty, false otherwise.</returns>
        public override bool Check()
        {
            if (Perceptions.Count == 0) return false;

            bool result = false;
            int idx = 0;
            while (result == false && idx < Perceptions.Count)
            {
                result = Perceptions[idx].Check();
                idx++;
            }
            return result;
        }
    }
}
