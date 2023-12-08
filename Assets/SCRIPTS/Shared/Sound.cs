using UnityEngine;
using GGG.Shared;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Game/Sound")]
    public class Sound : ScriptableObject
    {
        [Tooltip("Name of the sound clip.")]
        public string ClipName;
        [Tooltip("Types of audio source")]
        public SoundType AudioType;
        [Tooltip("Source of the sound")]
        public AudioClip AudioClip;
        [Tooltip("If the sound loops.")]
        public bool IsLoop;
        [Tooltip("If the sound plays when it's initialized.")]
        public bool PlayOnAwake;
        [Tooltip("Volume of the sound.")]
        [Range(0, 1)] public float Volume = 0.5f;

        //Source of the sound clip.
        [HideInInspector] public AudioSource Source;
    }
}
