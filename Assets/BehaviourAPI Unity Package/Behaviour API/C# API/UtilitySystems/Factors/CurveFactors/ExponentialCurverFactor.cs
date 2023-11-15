using System;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Create a curve factor with an exponential function
    /// </summary>
    public class ExponentialCurveFactor : CurveFactor
    {
        /// <summary>
        /// The exponent of the function
        /// </summary>
        public float Exponent = 1f;

        /// <summary>
        /// The x displacement of the function.
        /// </summary>
        public float DespX = 0f;

        /// <summary>
        /// The y displacement of the function.
        /// </summary>
        public float DespY = 0f;

        /// <summary>
        /// Set the exponent of the exponential curve factor.
        /// </summary>
        /// <param name="function">The new exponent.</param>
        /// <returns>The <see cref="ExponentialCurveFactor"/> itself. </returns>

        public ExponentialCurveFactor SetExponent(float exp)
        {
            Exponent = exp;
            return this;
        }

        /// <summary>
        /// Set the x displacement of the exponential curve factor.
        /// </summary>
        /// <param name="despX">The new x displacement.</param>
        /// <returns>The <see cref="ExponentialCurveFactor"/> itself. </returns>
        public ExponentialCurveFactor SetDespX(float despX)
        {
            DespX = despX;
            return this;
        }

        /// <summary>
        /// Set the y displacement of the exponential curve factor.
        /// </summary>
        /// <param name="despY">The new y displacement.</param>
        /// <returns>The <see cref="ExponentialCurveFactor"/> itself. </returns>
        public ExponentialCurveFactor SetDespY(float despY)
        {
            DespY = despY;
            return this;
        }

        /// <summary>
        /// Compute the utility using a exponential function [y = (x - dx)^exp + dy]
        /// </summary>
        /// <param name="x">The child utility. </param>
        /// <returns>The result of apply the function to <paramref name="x"/>.</returns>
        protected override float Evaluate(float x) => (float)Math.Pow(x - DespX, Exponent) + DespY;
    }
}
