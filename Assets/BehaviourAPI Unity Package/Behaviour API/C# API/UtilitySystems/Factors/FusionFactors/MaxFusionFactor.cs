using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Fusion factor that returns the higher utility from its children.
    /// </summary>
    public class MaxFusionFactor : FusionFactor
    {
        /// <summary>
        /// Returns the higher value from <paramref name="utilities"/>
        /// </summary>
        /// <param name="utilities">The child utilities.</param>
        /// <returns>The higher utility of the children.</returns>
        protected override float Evaluate(List<float> utilities)
        {
            return utilities.Max();
        }
    }
}
