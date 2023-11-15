using System;
using System.Collections.Generic;
using System.Text;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using System.Reflection;

    /// <summary>
    /// Composite node that selects one of its branch to execute it.
    /// </summary>
    public abstract class BranchNode : CompositeNode
    {
        BTNode m_SelectedNode;

        /// <summary>
        /// <inheritdoc/>
        /// Select a branch and starts it.
        /// </summary>
        public override void OnStarted()
        {
            base.OnStarted();

            int branchIndex = SelectBranchIndex();
            if (branchIndex < 0) branchIndex = 0;
            if (branchIndex >= ChildCount) branchIndex = ChildCount - 1;
            m_SelectedNode = GetBTChildAt(branchIndex);

            m_SelectedNode?.OnStarted();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stops the selected branch node.
        /// </summary>
        public override void OnStopped()
        {
            base.OnStopped();
            m_SelectedNode?.OnStopped();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the selected branch node.
        /// </summary>
        public override void OnPaused()
        {
            base.OnPaused();
            m_SelectedNode?.OnPaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the selected branch node.
        /// </summary>
        public override void OnUnpaused()
        {
            base.OnUnpaused();
            m_SelectedNode?.OnUnpaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Returns the status of its selected branch.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override Status UpdateStatus()
        {
            m_SelectedNode.OnUpdated();
            return m_SelectedNode?.Status ?? Status.Failure;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Override this method to define how to select the branch that will be executed.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected abstract int SelectBranchIndex();
    }
}
