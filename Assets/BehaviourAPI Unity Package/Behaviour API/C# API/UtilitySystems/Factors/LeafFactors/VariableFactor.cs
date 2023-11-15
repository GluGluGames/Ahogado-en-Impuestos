using System;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Leaf factor that computes its utility using a function.
    /// </summary>
    public class VariableFactor : LeafFactor
    {
        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The function used to get the utility
        /// </summary>
        public Func<float> Variable;

        /// <summary>
        /// The minimum value that <see cref="Variable"/> returns.
        /// </summary>
        public float min = 0f;

        /// <summary>
        /// The maximum variable that <see cref="Variable"/> returns.
        /// </summary>
        public float max = 1f;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Calculates the utility by normalizing the result of <see cref="Variable"/> 
        /// using <see cref="min"/> and <see cref="max"/>.
        /// </summary>
        /// <returns>The result of the function normalized.</returns>
        protected override float ComputeUtility()
        {
            Utility = Variable?.Invoke() ?? min;
            Utility = (Utility - min) / (max - min);
            return Utility;
        }

        #endregion
    }
}
