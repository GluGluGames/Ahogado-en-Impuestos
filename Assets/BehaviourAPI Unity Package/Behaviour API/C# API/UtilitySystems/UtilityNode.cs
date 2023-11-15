using System;

namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Base node for utility systems.
    /// </summary>
    public abstract class UtilityNode : Node
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public override Type GraphType => typeof(UtilitySystem);

        /// <summary>
        /// The utility value of the node.
        /// </summary>
        public float Utility
        {
            get => _utility;
            protected set
            {
                if (_utility != value)
                {
                    _utility = value;
                    UtilityChanged?.Invoke(_utility);
                }
            }
        }

        /// <summary>
        /// Event called when the node utility value changed
        /// </summary>
        public Action<float> UtilityChanged { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// If true, utility system update will recalculate <see cref="Utility"/>. If false, <see cref="Utility"/> can only be recalculate calling <see cref="UpdateUtility"/> manually.
        /// </summary>
        public bool PullingEnabled = true;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        float _utility;

        bool _needToRecalculateUtility;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public override object Clone()
        {
            UtilityNode node = (UtilityNode)base.Clone();
            node.UtilityChanged = (Action<float>)UtilityChanged?.Clone();
            return node;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Updates the current value of <see cref="Utility"/> if required.
        /// </summary>
        /// <param name="forceRecalculate">If true, the utility will be updated even if is not required.</param>
        public void UpdateUtility(bool forceRecalculate = false)
        {
            if (_needToRecalculateUtility || forceRecalculate)
            {
                Utility = GetUtility();
                _needToRecalculateUtility = false;
            }
        }

        /// <summary>
        /// Compute the utility of this node.
        /// </summary>
        /// <returns>The updated utility value of this node.</returns>
        protected abstract float GetUtility();

        /// <summary>
        /// If this node has <see cref="PullingEnabled"/> flag to true, indicates that the <see cref="Utility"/> has to be recalculated.
        /// </summary>
        public void MarkUtilityAsDirty()
        {
            if (PullingEnabled) _needToRecalculateUtility = true;
        }

        #endregion
    }
}