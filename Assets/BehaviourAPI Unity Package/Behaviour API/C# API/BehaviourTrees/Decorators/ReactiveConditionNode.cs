using System;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    
    using Core.Perceptions;

    /// <summary>
    /// Decorator that executes its child only if a perception is triggered. Perception is checked every frame
    /// and starts or stops the child execution when the value changes.
    /// </summary>
    public class ReactiveConditionNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception Perception;

        public ExecutionInterruptOptions InterruptOptions = ExecutionInterruptOptions.Stop;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        bool _childExecutedLastFrame;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ReactiveConditionNode SetPerception(Perception perception)
        {
            Perception = perception;
            return this;
        }

        public override object Clone()
        {
            var node = (ReactiveConditionNode)base.Clone();
            node.Perception = (Perception)Perception?.Clone();
            return node;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void OnStarted()
        {
            base.OnStarted();
            if (Perception != null) Perception.Initialize();
            else throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        public override void OnStopped()
        {
            base.OnStopped();
            if (_childExecutedLastFrame)
            {
                m_childNode.OnStopped();
                _childExecutedLastFrame = false;
            }

            if (Perception != null) Perception.Reset();
            else throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        public override void OnPaused()
        {   base.OnPaused();
            if (_childExecutedLastFrame)
            {
                m_childNode.OnPaused();
            }

            if (Perception != null) Perception.Pause();
            else throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        public override void OnUnpaused()
        {   
            base.OnUnpaused();
            if (_childExecutedLastFrame)
            {
                m_childNode.OnUnpaused();
            }

            if (Perception != null) Perception.Unpause();
            else throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        protected override Status UpdateStatus()
        {
            if (Perception != null)
            {
                if (m_childNode != null)
                {
                    if (Perception.Check())
                    {
                        if(!_childExecutedLastFrame)
                        {
                            if (m_childNode.Status == Status.None) m_childNode.OnStarted();
                            else if (m_childNode.Status == Status.Paused) m_childNode.OnUnpaused();
                        }
                        _childExecutedLastFrame = true;
                        m_childNode.OnUpdated();
                        return m_childNode.Status;
                    }
                    else
                    {
                        if (_childExecutedLastFrame)
                        {
                            if(InterruptOptions == ExecutionInterruptOptions.Stop) m_childNode.OnStopped();
                            else if (InterruptOptions == ExecutionInterruptOptions.Pause) m_childNode.OnPaused();
                        }
                        _childExecutedLastFrame = false;
                        return Status.Running;
                    }
                }
                throw new MissingChildException(this, "This decorator has no child");
            }
            throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        #endregion
    }
}
