using System;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using BehaviourTrees;
    using Core;

    /// <summary>
    /// Timer decorator that used unity time instead of system time..
    /// </summary>
    public class UnityTimerDecorator : DecoratorNode
    {
        /// <summary>
        /// The time that the decorator waits.
        /// </summary>
        public float TotalTime;

        float _currentTime;
        bool _childExecuted;

        public override void OnStarted()
        {
            base.OnStarted();
            _currentTime = 0f;
            _childExecuted = false;
        }

        /// <summary>
        /// Set the time that the decorator waits.
        /// </summary>
        /// <param name="time">The amount of time.</param>
        /// <returns>The decorator itself.</returns>
        public UnityTimerDecorator SetTotalTime(float time)
        {
            TotalTime = time;
            return this;
        }

        protected override Status UpdateStatus()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime < TotalTime) return Status.Running;

            if (m_childNode != null)
            {
                if (!_childExecuted)
                {
                    m_childNode.OnStarted();
                    _childExecuted = true;
                }
                m_childNode.OnUpdated();
                return m_childNode.Status;
            }
            throw new NullReferenceException("ERROR: Child node is not defined");
        }

        public override void OnStopped()
        {
            base.OnStopped();
            if (_childExecuted) m_childNode?.OnStopped();
        }

        public override void OnPaused()
        {
            if (_childExecuted) m_childNode?.OnPaused();
        }

        public override void OnUnpaused()
        {
            if (_childExecuted) m_childNode.OnUnpaused();
        }
    }

}
