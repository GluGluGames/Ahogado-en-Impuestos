using UnityEngine.Localization;
using UnityEngine;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Game/Resource")]
    public class Resource : ScriptableObject
    {
        [SerializeField] private LocalizedString Name;
        [SerializeField] private LocalizedString Description;
        [SerializeField] private Sprite Sprite;
        [SerializeField] private Sprite SelectedSprite;

        public string GetName() { return Name.GetLocalizedString(); }
        public string GetDescription() { return Description.GetLocalizedString(); }
        public Sprite GetSprite() { return Sprite; }

        public Sprite GetSelectedSprite() { return SelectedSprite; }
    }
}
