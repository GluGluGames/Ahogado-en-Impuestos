using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Fusion factor that returns the weighted average from its children utility.
    /// </summary>
    public class WeightedFusionFactor : FusionFactor
    {
        /// <summary>
        /// The weights applied to each utility.
        /// </summary>
        public float[] Weights = new float[0];

        /// <summary>
        /// Set the weights of the weighted fusion factor.
        /// </summary>
        /// <param name="weights">The new weights.</param>
        /// <returns>The <see cref="WeightedFusionFactor"/> itself. </returns>
        public WeightedFusionFactor SetWeights(params float[] weights)
        {
            Weights = weights;
            return this;
        }

        /// <summary>
        /// Set the weights of the weighted fusion factor.
        /// </summary>
        /// <param name="weights">The new weights.</param>
        /// <returns>The <see cref="WeightedFusionFactor"/> itself. </returns>
        public Factor SetWeights(IEnumerable<float> weights)
        {
            Weights = weights.ToArray();
            return this;
        }

        /// <summary>
        /// Returns the weighted average of the utilities in <paramref name="utilities"/>
        /// </summary>
        /// <param name="utilities">The child utilities.</param>
        /// <returns>The weighted average of the children utility.</returns>
        protected override float Evaluate(List<float> utilities)
        {
            return utilities.Zip(Weights, (utility, weight) => utility * weight).Sum();
        }

        public override object Clone()
        {
            WeightedFusionFactor fusion = (WeightedFusionFactor)base.Clone();
            fusion.Weights = Weights.ToArray();
            return fusion;
        }
    }
}
