using System;

namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// Branch selector node that select the branch by a function that returns an index
    /// </summary>
    public class FunctionBranchNode : BranchNode
    {
        /// <summary>
        /// The function used to get the branch index. The result will be clamped between 0 and child count.
        /// </summary>
        public Func<int> nodeIndexFunction;

        /// <summary>
        /// Set the function used to get the branch index.
        /// </summary>
        /// <param name="nodeIndexFunction">The value of the function.</param>
        /// <returns>The <see cref="FunctionBranchNode"/> itself.</returns>
        public FunctionBranchNode SetNodeIndexFunction(Func<int> nodeIndexFunction)
        {
            this.nodeIndexFunction = nodeIndexFunction;
            return this;
        }

        protected override int SelectBranchIndex()
        {
            int index = nodeIndexFunction?.Invoke() ?? 0;
            return index;
        }
    }
}
