using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Change the sprite of the renderer component. 
    /// If time is bigger that 0, the action will end when the time passes and
    /// revert the sprite value. Otherwise the action will end inmediatly.
    /// </summary>
    [SelectionGroup("SPRITE")]
    public class ChangeSpriteAction : UnityAction
    {
        /// <summary>
        /// The sprite applied to the renderer.
        /// </summary>
        [SerializeField] Sprite sprite;

        /// <summary>
        /// The time to finish the action and revert the value.
        /// </summary>
        [SerializeField] float time = 1f;

        private Sprite _previousSprite;
        private float _currentTime;

        public override void Start()
        {
            _previousSprite = context.Renderer.Sprite;
            context.Renderer.Sprite = sprite;
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
                context.Renderer.Sprite = _previousSprite;
            }
        }

        public override string ToString()
        {
            if(time > 0f)
            {
                return $"Change renderer sprite to {sprite} by {time} second(s)";
            }
            else
            {
                return $"Change renderer sprite to {sprite}";
            }            
        }
    }
}
