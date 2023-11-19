using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Action that changes the volume of the sound component.
    /// </summary>
    public class SetVolumeAction : UnityAction
    {
        /// <summary>
        /// The value used to modify the volume
        /// </summary>
        [Range(0f, 1f)] public float volume;

        /// <summary>
        /// The value should multiply or set directly?
        /// </summary>
        public bool IsRelative;

        public override void Start()
        {
            context.Sound.Volume = IsRelative ? context.Sound.Volume * volume : volume;
        }

        public override Status Update()
        {
            return Status.Success;
        }
    }
}
