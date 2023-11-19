using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public interface ISoundComponent
    {
        public float Volume { get; set; }
        public void StartSound(AudioClip clip);

        public bool IsPlayingSound();

        public void CancelSound();

    }
}
