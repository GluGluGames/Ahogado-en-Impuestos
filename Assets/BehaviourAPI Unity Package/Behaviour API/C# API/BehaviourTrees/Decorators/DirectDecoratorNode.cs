namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core;    

    /// <summary>
    /// Decorator that always execute its child.
    /// </summary>
    public abstract class DirectDecoratorNode : DecoratorNode
    {
        /// <summary>
        /// <inheritdoc/>
        /// Starts the execution of its child.
        /// </summary>
        /// <exception cref="MissingChildException">If child is null.</exception>
        public override void OnStarted()
        {
            base.OnStarted();

            if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

            m_childNode.OnStarted();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stops the execution of its child.
        /// </summary>
        /// <exception cref="MissingChildException">If child is null.</exception>
        public override void OnStopped()
        {
            base.OnStopped();

            if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

            m_childNode.OnStopped();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Updates the execution of its child and returns the value modified.
        /// </summary>
        /// <exception cref="MissingChildException">If child is null.</exception>
        protected override Status UpdateStatus()
        {
            if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

            m_childNode.OnUpdated();
            var status = m_childNode.Status;
            return ModifyStatus(status);
        }

        /// <summary>
        /// Gets the children status and return it modified.
        /// </summary>
        /// <param name="childStatus">The child current status.</param>
        /// <returns>The child status modified.</returns>
        protected abstract Status ModifyStatus(Status childStatus);

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the child node.
        /// </summary>
        /// <exception cref="MissingChildException">If child is null.</exception>
        public override void OnPaused()
        {
            base.OnPaused();

            if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

            m_childNode.OnPaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the child node.
        /// </summary>
        /// <exception cref="MissingChildException">If child is null.</exception>
        public override void OnUnpaused()
        {
            base.OnUnpaused();

            if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

            m_childNode.OnUnpaused();
        }
    }
}
