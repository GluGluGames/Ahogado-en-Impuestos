using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    using Core;
    

    /// <summary>
    /// Factor that modifies its child value with a function.
    /// </summary>  
    public abstract class CurveFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => 1;

        #endregion

        Factor m_childFactor;

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the child factor of this curve factor.
        /// </summary>
        /// <param name="factor">The new child factor. </param>
        /// <exception cref="MissingChildException">If factor is null.</exception>
        protected internal void SetChild(Factor factor)
        {
            if(factor != null)
            {
                m_childFactor = factor;
            }
            else
            {
                throw new MissingChildException(this, "Can't set null node as child");
            }
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is Factor factor)
                m_childFactor = factor;
            else
                throw new MissingChildException(this, "This function factor has no child, or it's type is incorrect.");
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Compute its utility applying a function to the utility of its child factor.
        /// </summary>
        /// <returns>The result of apply the evaluate function to the child, or 0 if the child is null.</returns>
        protected override float ComputeUtility()
        {
            m_childFactor?.UpdateUtility();
            return Evaluate(m_childFactor?.Utility ?? 0f);
        }

        /// <summary>
        /// Modify the child utility with a function.
        /// </summary>
        /// <param name="childUtility">The child utility.</param>
        /// <returns>The child utility modified.</returns>
        protected abstract float Evaluate(float childUtility);


        /// <summary>
        /// Test the evaluation function.
        /// </summary>
        /// <param name="x">The entry value of the function.</param>
        /// <returns>The result of the function.</returns>
        public float TestEvaluate(float x) => Evaluate(x);
        #endregion
    }
}
