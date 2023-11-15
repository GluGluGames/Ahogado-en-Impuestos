using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Fusion factor that returns the lower utility from its children.
    /// </summary>
    public class MinFusionFactor : FusionFactor
    {
        /// <summary>
        /// Returns the lower value from <paramref name="utilities"/>
        /// </summary>
        /// <param name="utilities">The child utilities.</param>
        /// <returns>The lower utility of the children.</returns>
        protected override float Evaluate(List<float> utilities)
        {
            return utilities.Min();
        }
    }
}
