using UnityEngine.Localization;
using UnityEngine;

namespace GGG.Shared
{
    public abstract class Resource : ScriptableObject
    {
        [SerializeField] private LocalizedString Name;
        [SerializeField] private LocalizedString Description;
        [SerializeField] private Sprite Sprite;
        [SerializeField] private Sprite SelectedSprite;

        public LocalizedString GetName() { return Name; }
        public LocalizedString GetDescription() { return Description; }
        public Sprite GetSprite() { return Sprite; }

        public Sprite GetSelectedSprite() { return SelectedSprite; }
    }
}
