namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    

    /// <summary>
    /// Node that execute its child node the number of times determined by <see cref="Iterations"/>
    /// </summary>
    public  class LoopNode : DirectDecoratorNode
    {
        #region ----------------------------------------- Properties -----------------------------------------

        int _currentIterations;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The number of times that the child node should end its execution to end the decorator.
        /// if its value is -1 this number is infinite.
        /// </summary>
        public int Iterations = -1;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the <see cref="Iterations"/> value to <paramref name="iterations"/>.
        /// </summary>
        /// <param name="iterations">The new iterations value.</param>
        /// <returns>The <see cref="LoopNode"/> itself.</returns>
        public LoopNode SetIterations(int iterations)
        {
            Iterations = iterations;  
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Reset the current iterations.
        /// </summary>
        public override void OnStarted()
        {
            base.OnStarted();
            _currentIterations = 0;
        }

        /// <summary>
        /// <inheritdoc/> If the child execution ends increase the current iteration count. If this count reaches <see cref="Iterations"/>, 
        /// return the status of the child, otherwise restart the child and return Running.
        /// </summary>
        /// <param name="childStatus">The current child status.</param>
        /// <returns>Running if the iteration count is not reached, <paramref name="childStatus"/> otherwise.</returns>
        protected override Status ModifyStatus(Status childStatus)
        {
            // If child execution ends, restart until currentIterations > Iterations
            if (childStatus != Status.Running)
            {
                _currentIterations++;
                if (Iterations == -1 || _currentIterations < Iterations)
                {
                    childStatus = Status.Running;
                    m_childNode.OnStopped();
                    m_childNode.OnStarted();
                }
            }
            return childStatus;
        }

        #endregion
    }
}
