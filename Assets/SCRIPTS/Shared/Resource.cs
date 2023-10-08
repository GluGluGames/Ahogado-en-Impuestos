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
        [SerializeField] private Resources Type;

        public LocalizedString GetName() { return Name; }
        public LocalizedString GetDescription() { return Description; }
        public Sprite GetSprite() { return Sprite; }
        public Resources GetResource() { return Type; }
    }
}
