using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that delays the execution. 
    /// </summary>
    public class DelayAction : UnityAction
    {
        /// <summary>
        /// The time the action waits to end in seconds.
        /// </summary>
        public float delayTime;

        float _currentTime;

        /// <summary>
        /// Create a new DelayAction.
        /// </summary>
        public DelayAction()
        {
        }

        /// <summary>
        /// Create a new DelayAction.
        /// </summary>
        /// <param name="delayTime">The time the action waits to end in seconds.</param>
        public DelayAction(float delayTime)
        {
            this.delayTime = delayTime;
        }

        public override string ToString() => $"Wait {delayTime} second(s)";

        public override void Start()
        {
            _currentTime = 0;
        }

        public override Status Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > delayTime)
            {
                return Status.Success;
            }
            else
                return Status.Running;
        }
    }
}
