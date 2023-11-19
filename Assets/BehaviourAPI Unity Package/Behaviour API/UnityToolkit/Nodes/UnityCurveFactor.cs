using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using UtilitySystems;

    /// <summary>
    /// Create the function using unity animation curve
    /// </summary>
    public class UnityCurveFactor : CurveFactor
    {
        /// <summary>
        /// The curve that represents the function.
        /// </summary>
        public AnimationCurve curve = new AnimationCurve();

        protected override float Evaluate(float childUtility)
        {
            return curve.Evaluate(childUtility);
        }

        public UnityCurveFactor SetCurve(AnimationCurve animationCurve)
        {
            curve = animationCurve;
            return this;
        }
    }
}
