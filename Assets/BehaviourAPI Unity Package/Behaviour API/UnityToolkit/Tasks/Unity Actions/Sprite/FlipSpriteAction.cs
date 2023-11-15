using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Flip the sprite of the renderer component. 
    /// If time is bigger that 0, the action will end when the time passes and
    /// revert the flip. Otherwise the action will end inmediatly.
    /// </summary>
    [SelectionGroup("SPRITE")]
    public class FlipSpriteAction : UnityAction
    {
        /// <summary>
        /// ¿Flip the x coord?
        /// </summary>
        [SerializeField] bool flipx;

        /// <summary>
        /// ¿Flip the y coord?
        /// </summary>
        [SerializeField] bool flipy;

        /// <summary>
        /// The time to finish the action and revert the value.
        /// </summary>
        [SerializeField] float time = 1f;
        
        private float _currentTime;
        private bool previousFlipX;
        private bool previousFlipY;

        public override void Start()
        {
            previousFlipX = context.Renderer.FlipX;
            previousFlipY = context.Renderer.FlipY;

            context.Renderer.FlipX = flipx;
            context.Renderer.FlipY = flipy;
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
            if (time > 0f)
            {
                context.Renderer.FlipX = previousFlipX;
                context.Renderer.FlipY = previousFlipY;
            }
        }

        public override string ToString()
        {
            if (time > 0f)
            {
                return $"Change renderer sprite by {time} second(s)";
            }
            else
            {
                return $"Flip renderer sprite";
            }
        }
    }
}
