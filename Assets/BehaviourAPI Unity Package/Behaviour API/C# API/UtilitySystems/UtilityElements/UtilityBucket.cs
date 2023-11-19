using System;
using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    using Core;
    

    /// <summary>
    /// Utility element that handle multiple <see cref="UtilitySelectableNode"/> itself and
    /// returns the maximum utility if its best candidate utility is higher than the threshold.
    /// </summary>
    public class UtilityBucket : UtilitySelectableNode
    {
        #region ----------------------------------------- Properties -----------------------------------------

        public override Type ChildType => typeof(UtilitySelectableNode);
        public override int MaxOutputConnections => -1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public float Inertia = 1.3f;

        /// <summary>
        /// The minimum value of utility that the actions of the bucket must have to execute it with priority.
        /// </summary>
        public float BucketThreshold = .3f;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        List<UtilitySelectableNode> _utilityCandidates = new List<UtilitySelectableNode>();

        UtilitySelectableNode _currentBestElement;
        UtilitySelectableNode _lastExecutedElement;

        #endregion       

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Add a utility selectable node to the bucket.
        /// </summary>
        /// <param name="elem">The new selectable node.</param>
        /// <exception cref="MissingChildException">If <paramref name="elem"/> is null.</exception>
        protected internal void AddElement(UtilitySelectableNode elem)
        {
            if (elem != null)
            {
                _utilityCandidates.Add(elem);
            }
            else
            {
                throw new MissingChildException(this, $"Can't add null selectable node.");
            }
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);
            _utilityCandidates = new List<UtilitySelectableNode>();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is UtilitySelectableNode s)
                    _utilityCandidates.Add(s);
                else
                    throw new MissingChildException(this, $"The child {children[i]} is null or missing type.");
            }
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="MissingChildException">If utility candidate list is empty.</exception>
        public override void OnStarted()
        {
            base.OnStarted();

            if (_utilityCandidates.Count == 0)
                throw new MissingChildException(this, "The list of utility candidates of this bucket is empty.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// The utility will be the max utility of the actions in this group.
        /// Also updates the current best action.
        /// </summary>
        /// <returns>The maximum utility of the actions in the group or 0 if is empty.</returns>
        protected override float GetUtility()
        {
            _currentBestElement = ComputeCurrentBestAction();
            var maxUtility = _currentBestElement?.Utility ?? -1f;
            // if utility is higher than the bucket threshold, enable the priority.
            ExecutionPriority = maxUtility > BucketThreshold;
            return maxUtility;
        }

        private UtilitySelectableNode ComputeCurrentBestAction()
        {
            float currentHigherUtility = -1f; // If value starts in 0, elems with Utility == 0 couldn't be executed

            UtilitySelectableNode newBestElement = null;

            int i = 0;
            var currentElementIsLocked = false; // Set to true when a the current element is a locked bucket

            while (i < _utilityCandidates.Count && !currentElementIsLocked)
            {
                // Update utility:
                var currentCandidate = _utilityCandidates[i];

                if (currentCandidate == null) continue;

                currentCandidate.UpdateUtility();

                var utility = currentCandidate.Utility;
                if (currentCandidate == _currentBestElement) utility *= Inertia;

                // If it's higher than the current max utility, update the selected element.
                if (utility >= BucketThreshold && utility > currentHigherUtility)
                {
                    newBestElement = currentCandidate;
                    currentHigherUtility = utility;
                }

                // If the current candidate is a locked bucket:
                if (currentCandidate.ExecutionPriority) currentElementIsLocked = true;

                i++;
            }

            return newBestElement;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Execute the current best action. If the best action changes, stops the last
        /// best action and starts the new one.
        /// </summary>
        public override void OnUpdated()
        {
            if (_currentBestElement != _lastExecutedElement)
            {
                _lastExecutedElement?.OnStopped();
                _lastExecutedElement = _currentBestElement;
                _lastExecutedElement?.OnStarted();
            }

            if (_lastExecutedElement != null)
            {
                _lastExecutedElement.OnUpdated();
                Status = _lastExecutedElement.Status;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// Stop the last executed action.
        /// </summary>
        public override void OnStopped()
        {
            base.OnStopped();
            _lastExecutedElement?.OnStopped();
            _lastExecutedElement = null;
            _currentBestElement = null;
        }

        public override void OnPaused()
        {
            _lastExecutedElement?.OnPaused();
        }

        public override void OnUnpaused()
        {
            _lastExecutedElement?.OnUnpaused();
        }

        #endregion
    }
}
