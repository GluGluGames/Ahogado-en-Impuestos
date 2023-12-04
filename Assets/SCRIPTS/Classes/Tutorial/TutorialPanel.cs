using UnityEngine;
using UnityEngine.Localization;

namespace GGG.Classes.Tutorial
{
    [System.Serializable]
    public class TutorialPanel
    {
        [SerializeField] private LocalizedString Title;
        [SerializeField] private Sprite Image;
        [SerializeField] private LocalizedString Text;

        public string GetTitle() => Title.GetLocalizedString();
        public Sprite GetImage() => Image;
        public string GetText() => Text.GetLocalizedString();
    }
}
