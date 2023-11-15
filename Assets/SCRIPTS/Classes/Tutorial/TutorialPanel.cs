using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [System.Serializable]
    public class TutorialPanel
    {
        [SerializeField] private string Title;
        [SerializeField] private Sprite Image;
        [SerializeField, TextArea] private string Text;

        public string GetTitle() => Title;
        public Sprite GetImage() => Image;
        public string GetText() => Text;
    }
}
