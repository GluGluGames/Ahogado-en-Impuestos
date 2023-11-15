using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Change the alpha value of the renderer component. 
    /// If time is bigger that 0, the action will end when the time passes and
    /// revert the alpha value. Otherwise the action will end inmediatly.
    /// </summary>
    [SelectionGroup("SPRITE")]
    public class ChangeAlphaAction : UnityAction
    {
        /// <summary>
        /// The value of the alpha
        /// </summary>
        [SerializeField, Range(0f, 1f)] float alpha;

        /// <summary>
        /// The time to finish the action and revert the value.
        /// </summary>
        [SerializeField] float time = 1f;

        private float _previousAlpha;
        private float _currentTime;

        public override void Start()
        {
            _previousAlpha = context.Renderer.Tint.a;
            Color color = context.Renderer.Tint;
            color.a = alpha;
            context.Renderer.Tint = color;
        }

        public override Status Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > time)
            {
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }

        public override void Stop()
        {
            if(time > 0f)
            {
                Color color = context.Renderer.Tint;
                color.a = _previousAlpha;
                context.Renderer.Tint = color;
            }
        }

        public override string ToString()
        {
            if (time > 0f)
            {
                return $"Change renderer alpha to {alpha} by {time} second(s)";
            }
            else
            {
                return $"Change renderer alpha to {alpha}";
            }
        }
    }
}
