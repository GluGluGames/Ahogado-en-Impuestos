using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// BTNode subtype that has multiple children and executes them according to certain conditions.
    /// </summary>
    public abstract class CompositeNode : BTNode
    {
        private static Random rng = new Random();

        #region ------------------------------------------ Properties -----------------------------------------

        public sealed override int MaxOutputConnections => -1;

        /// <summary>
        /// List of <see cref="BTNode"/> children.
        /// </summary>
        protected List<BTNode> m_children = new List<BTNode>();

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// True if the execution order of the childs will randomize on start.
        /// </summary>
        public bool IsRandomized;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Add a new child to <see cref="m_children"/> list.
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="MissingChildException">If <paramref name="child"/> is null."/></exception>
        protected internal void AddChild(BTNode child)
        {
            if(child != null) m_children.Add(child);
            else throw new MissingChildException(this, "Can't add null node as child");
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            m_children = new List<BTNode>();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is BTNode t)
                    m_children.Add(t);
                else
                    throw new MissingChildException(this, $"child {i} is not BTNode");
            }
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// <para>Suffle the child list if <see cref="IsRandomized"/> is true.</para>
        /// </summary>
        /// <exception cref="MissingChildException">If the <see cref="m_children"/> list has no elements.</exception>
        public override void OnStarted()
        {
            base.OnStarted();

            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");

            if (IsRandomized) m_children = m_children.OrderBy(elem => rng.NextDouble()).ToList();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="MissingChildException">If the <see cref="m_children"/> list has no elements.</exception>
        public override void OnStopped()
        {
            base.OnStopped();

            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="MissingChildException">If the <see cref="m_children"/> list has no elements.</exception>
        public override void OnPaused()
        {
            base.OnPaused();

            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="MissingChildException">If the <see cref="m_children"/> list has no elements.</exception>
        public override void OnUnpaused()
        {
            base.OnUnpaused();

            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");
        }

        /// <summary>
        /// Get the element in btnode children list at the given index.
        /// </summary>
        /// <param name="idx">The index in the list.</param>
        /// <returns></returns>
        /// <exception cref="MissingChildException">If <see cref="m_children"/> is empty or if the index is out of bounds.</exception>
        protected BTNode GetBTChildAt(int idx)
        {
            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");
            if (idx < 0 || idx >= m_children.Count) throw new MissingChildException(this, "This composite has no child at index " + idx);
            
            return m_children[idx];
        }

        /// <summary>
        /// <inheritdoc/>
        /// Resets its children too.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override bool ResetLastStatus()
        {
            bool b = base.ResetLastStatus();
            if(b) m_children.ForEach(child => child.ResetLastStatus());
            return b;
        }

        #endregion
    }
}