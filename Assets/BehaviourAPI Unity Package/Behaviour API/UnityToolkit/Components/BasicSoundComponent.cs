using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public class BasicSoundComponent : MonoBehaviour, ISoundComponent
    {
        [SerializeField] private AudioSource m_AudioSource;
        public void CancelSound()
        {
           m_AudioSource.Stop();
        }

        public bool IsPlayingSound()
        {
            return m_AudioSource.isPlaying;
        }

        public void StartSound(AudioClip clip)
        {
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }

        public float Volume { get =>  m_AudioSource.volume; set=>  m_AudioSource.volume = value; }
    }
}
