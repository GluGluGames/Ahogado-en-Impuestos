using System;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Create a curve factor with a custom function
    /// </summary>
    public class CustomCurveFactor : CurveFactor
    {
        /// <summary>
        /// The function used to calculate the utility
        /// </summary>
        public Func<float, float> Function;

        /// <summary>
        /// Set the function of the custom curve factor.
        /// </summary>
        /// <param name="function">The new function.</param>
        /// <returns>The <see cref="CustomCurveFactor"/> itself. </returns>
        public CustomCurveFactor SetFunction(Func<float, float> function)
        {
            Function = function;
            return this;
        }

        /// <summary>
        /// Compute the utility using the function stored in <see cref="Function"/>. 
        /// If its null, return the child utility.
        /// </summary>
        /// <param name="x">The child utility. </param>
        /// <returns>The result of apply the function to <paramref name="x"/>.</returns>
        protected override float Evaluate(float x) => Function?.Invoke(x) ?? x;
    }
}
