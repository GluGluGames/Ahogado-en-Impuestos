using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Game/Achievement")]
    public class Achievement : ScriptableObject
    {
        [SerializeField] private string Key;
        [SerializeField] private LocalizedString Name;
        [SerializeField] private LocalizedString Description;
        [SerializeField] private Sprite Icon;
        [SerializeField] private bool Hide;
        [SerializeField] private bool Unlocked;

        public string GetKey() => Key;
        public string GetName() => Name.GetLocalizedString();
        public string GetDescription() => Description.GetLocalizedString();
        public Sprite GetSprite() => Icon;
        public bool IsHidden() => Hide;

        public bool IsUnlocked()
        {
            if (PlayerPrefs.HasKey(Key)) return PlayerPrefs.GetInt(Key) == 1;

            return Unlocked;
        }

        public void Unlock() => PlayerPrefs.SetInt(Key, 1);
    }
}
