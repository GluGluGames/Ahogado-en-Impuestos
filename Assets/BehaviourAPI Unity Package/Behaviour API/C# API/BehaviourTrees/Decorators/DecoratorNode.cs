using System.Collections.Generic;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;
        

    /// <summary>
    /// BTNode that alters the result returned by its child node or its execution.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public sealed override int MaxOutputConnections => 1;

        /// <summary>
        /// The behaviour tree child node.
        /// </summary>
        protected BTNode m_childNode;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        
        /// <summary>
        /// Set the current child node (only used internally).
        /// </summary>
        /// <param name="child">The new child node.</param>
        /// <exception cref="MissingChildException">If <paramref name="child"/> is null</exception>
        protected internal void SetChild(BTNode child)
        {
            if (child != null) m_childNode = child;
            else throw new MissingChildException(this, "Can't set null node as child");
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is BTNode bTNode)
                m_childNode = bTNode;
            else
                throw new MissingChildException(this, $"Child is null or is not BTNode");
        }

        #endregion

        #region ------------------------------------- Runtime methods ------------------------------------
        
        public override bool ResetLastStatus()
        {
            bool b = base.ResetLastStatus();
            if(b) m_childNode.ResetLastStatus();
            return b;
        }

        #endregion
    }
}