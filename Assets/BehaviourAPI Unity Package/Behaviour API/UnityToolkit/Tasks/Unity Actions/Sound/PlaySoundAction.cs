using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;
    /// <summary>
    /// Action that plays an specified sound and finish when the clip ends.
    /// </summary>
    [SelectionGroup("Sound")]
    public class PlaySoundAction : UnityAction
    {
        /// <summary>
        /// The played clip
        /// </summary>
        [SerializeField] AudioClip clip;

        public override void Start()
        {
            context.Sound.StartSound(clip);
        }

        public override Status Update()
        {
            if (context.Sound.IsPlayingSound())
            {
                return Status.Running;
            }
            else
            {
                return Status.Success;
            }
        }

        public override void Stop()
        {
            context.Sound.CancelSound();
        }

        public override string ToString() => $"Play clip {clip}";
    }

}