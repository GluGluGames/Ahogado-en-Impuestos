using System;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Create a curve factor with an sigmoid function
    /// </summary>
    public class SigmoidCurveFactor : CurveFactor
    {
        /// <summary>
        /// The grown rate of the function.
        /// </summary>
        public float GrownRate = 1f;

        /// <summary>
        /// The mid point of the function.
        /// </summary>
        public float Midpoint = 0.5f;

        /// <summary>
        /// Set the grown rate of the sigmoid curve factor.
        /// </summary>
        /// <param name="grownRate">The new grown rate.</param>
        /// <returns>The <see cref="SigmoidCurveFactor"/> itself. </returns>
        public SigmoidCurveFactor SetGrownRate(float grownRate)
        {
            GrownRate = grownRate;
            return this;
        }

        /// <summary>
        /// Set the mid point of the sigmoid curve factor.
        /// </summary>
        /// <param name="grownRate">The new mid point.</param>
        /// <returns>The <see cref="SigmoidCurveFactor"/> itself. </returns>
        public SigmoidCurveFactor SetMidpoint(float midpoint)
        {
            Midpoint = midpoint;
            return this;
        }

        /// <summary>
        /// Compute the utility using a exponential function [y = (1 / (1 + e^(-grownRate * (x - midPoint)))]
        /// </summary>
        /// <param name="x">The child utility. </param>
        /// <returns>The result of apply the function to <paramref name="x"/>.</returns>
        protected override float Evaluate(float x) => (float)(1f / (1f + Math.Pow(Math.E, -GrownRate * (x - Midpoint))));
    }
}
