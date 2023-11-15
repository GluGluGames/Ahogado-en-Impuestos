using System.Collections.Generic;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Compound perception that returns true when all the subperception return true.
    /// </summary>
    public class AndPerception : CompoundPerception
    {
        /// <summary>
        /// Create a new and perception.
        /// </summary>
        public AndPerception() : base() { }

        /// <summary>
        /// Create a new and perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public AndPerception(List<Perception> perceptions) : base(perceptions) { }

        /// <summary>
        /// Create a new and perception.
        /// </summary>
        /// <param name="perceptions">The list of subperceptions.</param>
        public AndPerception(params Perception[] perceptions) : base(perceptions) { }

        /// <summary>
        /// Check all the sub perceptions and return true only if all of them returned true.
        /// Returns false if the sub perception list is empty.
        /// </summary>
        /// <returns>true if all of the sub perceptions returned true and the sub perception list is not empty, false otherwise.</returns>
        public override bool Check()
        {
            if(Perceptions.Count == 0) return false;

            bool result = true;
            int idx = 0;
            while(result == true && idx < Perceptions.Count)
            {
                result = Perceptions[idx].Check();
                idx++;
            }
            return result;
        }
    }
}
