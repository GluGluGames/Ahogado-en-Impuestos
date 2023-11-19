using System;
using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    
    using Core;

    /// <summary>
    /// Utility selectable node that computes its utility with a factor.
    /// </summary>
    public abstract class UtilityExecutableNode : UtilitySelectableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------

        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor _factor;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the child factor of the node.
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <exception cref="MissingChildException">If factor is null.</exception>
        protected internal void SetFactor(Factor factor)
        {
            if(factor != null)
            {
                _factor = factor;
            }
            else
            {
                throw new MissingChildException(this, "The child factor can't be null.");
            }            
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is Factor f)
                _factor = f;
            else
                throw new MissingChildException(this, "The child factor can't be null.");
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Gets the utility from its child
        /// </summary>
        /// <returns>The child factor utility.</returns>
        protected override float GetUtility()
        {
            _factor?.UpdateUtility();
            return _factor?.Utility ?? 0f;
        }

        #endregion
    }
}
