using UnityEngine;
using GGG.Shared;
using UnityEngine.Audio;
using System;

namespace GGG
{
    public class SoundManager : MonoBehaviour
    {
        #region Instance

        [Tooltip("Instance of SoundManager, so it can be accessed from other classes.")]
        public static SoundManager Instance;

        #endregion

        #region Private Fields

        [Tooltip("Group of general sounds. SerializeField: you can change it from the editor.")]
        [SerializeField] private AudioMixerGroup GeneralMixerGroup;
        [Tooltip("Group of Music type sounds. SerializeField: you can change it from the editor.")]
        [SerializeField] private AudioMixerGroup MusicMixerGroup;
        [Tooltip("Group of SoundEffect type sounds. SerializeField: you can change it from the editor.")]
        [SerializeField] private AudioMixerGroup SoundEffectsMixerGroup;
        [Tooltip("Array of sounds. SerializeField: you can change it from the editor.")]
        [SerializeField] private Sound[] Sounds;

        private bool _isMusicActive = true;

        #endregion

        #region Unity Events

        /// <summary>
        /// Initializes Instance. Calls for it to not be destroyed when loading new scenes.
        /// It initializes each sound in _sounds, establishes their type, an plays them if playOnAwake is true.
        /// </summary>
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (Sound s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.AudioClip;
                s.Source.loop = s.IsLoop;
                s.Source.volume = s.Volume;

                switch (s.AudioType)
                {
                    case SoundType.SoundEffect:
                        s.Source.outputAudioMixerGroup = SoundEffectsMixerGroup;
                        break;

                    case SoundType.Music:
                        s.Source.outputAudioMixerGroup = MusicMixerGroup;
                        break;
                }

                if (s.PlayOnAwake)
                    s.Source.Play();
            }
        }

        private void OnValidate()
        {
            Sounds = Resources.LoadAll<Sound>("Sounds");
        }

        #endregion

        #region Methods
        
        public void SetMusicVolume(float volume)
        {
            MusicMixerGroup.audioMixer.SetFloat("Music", volume);
        }
        public void SetMusicActive(bool active)
        {
            _isMusicActive = active;
        }
        public bool GetMusicActive()
        {
            return _isMusicActive;
        }

        /// <summary>
        /// Finds the clip named "clipname" and plays it if it exists.
        /// </summary>
        /// <param name="clipName">Name of the clip to Play.</param>
        public void Play(string clipName)
        {
            Sound s = Array.Find(Sounds, dummySound => dummySound.ClipName == clipName);
            if (s == null)
            {
                Debug.LogError("Sound " + clipName + " not found");
                return;
            }
            s.Source.Play();
        }

        public void Play(Sound sound)
        {
            Play(sound.name);
        }

        public bool IsPlaying(string clipName)
        {
            Sound s = Array.Find(Sounds, x => x.ClipName == clipName);
            if (!s)
                throw new Exception("Not sound found");

            return s.Source.isPlaying;
        }
        

        public void Resume(string clipName)
        {
            Sound s = Array.Find(Sounds, dummySound => dummySound.ClipName == clipName);
            if (s == null)
            {
                Debug.LogError("Sound " + clipName + " not found");
                return;
            }
            s.Source.UnPause();
        }

        public void Pause(string clipName)
        {
            Sound s = Array.Find(Sounds, dummySound => dummySound.ClipName == clipName);
            if (s == null)
            {
                Debug.LogError("Sound " + clipName + " not found");
                return;
            }
            s.Source.Pause();
        }

        /// <summary>
        /// Finds the clip named "clipname" and stops it if it exists.
        /// </summary>
        /// <param name="clipName">Name of the clip to Stop.</param>
        public void Stop(string clipName)
        {
            Sound s = Array.Find(Sounds, dummySound => dummySound.ClipName == clipName);
            if (s == null)
            {
                Debug.LogError("Sound " + clipName + " not found");
                return;
            }
            s.Source.Stop();
        }

        #endregion

    }
}
