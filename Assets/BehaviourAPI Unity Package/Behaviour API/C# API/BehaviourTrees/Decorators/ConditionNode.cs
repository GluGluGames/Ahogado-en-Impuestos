using System;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using Core.Perceptions;

    /// <summary>
    /// Decorator that executes its child only if a perception is triggered. Perception is checked at the start
    /// and return Failure if isn't triggered. Otherwise execute the child and returns its value.
    /// </summary>
    public class ConditionNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        bool _executeChild;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public Perception Perception;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ConditionNode SetPerception(Perception perception)
        {
            Perception = perception;
            return this;
        }

        public override object Clone()
        {
            var node = (ConditionNode)base.Clone();
            node.Perception = (Perception)Perception?.Clone();
            return node;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void OnStarted()
        {
            base.OnStarted();
            if (Perception != null)
            {
                Perception.Initialize();
                _executeChild = Perception.Check();
                Perception.Reset();
            }
            else
                throw new NullReferenceException("ERROR: Perception is not defined.");

            if (_executeChild)
            {
                m_childNode.OnStarted();
            }
        }

        protected override Status UpdateStatus()
        {
            if (_executeChild)
            {
                if (m_childNode == null) throw new MissingChildException(this, "This decorator has no child");

                m_childNode.OnUpdated();
                return m_childNode.Status;
            }
            else
            {
                return Status.Failure;
            }
        }

        public override void OnStopped()
        {
            base.OnStopped();
            if (_executeChild)
            {
                m_childNode.OnStopped();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the child node if it is running.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void OnPaused()
        {

            base.OnPaused();
            if (_executeChild)
            {
                m_childNode.OnPaused();
            }
        }

        /// <summary>
        /// Unpauses the child node if it is running.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void OnUnpaused()
        {
            base.OnUnpaused();
            if (_executeChild)
            {
                m_childNode.OnUnpaused();
            }
        }

        public override void SetExecutionContext(ExecutionContext context)
        {
            Perception?.SetExecutionContext(context);
        }

        #endregion
    }
}
