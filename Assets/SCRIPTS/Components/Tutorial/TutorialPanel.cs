using UnityEngine;
using UnityEngine.Localization;

namespace GGG.Components.Tutorial
{
    [System.Serializable]
    public class TutorialPanel
    {
        [SerializeField] private LocalizedString Title;
        [SerializeField] private Sprite Image;
        [SerializeField] private LocalizedString Text;
        [SerializeField] private TutorialObjective Objectives;
        [SerializeField] private bool Close;
        [SerializeField] private bool Finish;

        public string GetTitle() => Title.GetLocalizedString();
        public Sprite GetImage() => Image;
        public string GetText() => Text.GetLocalizedString();
        public TutorialObjective GetObjectives() => Objectives;
        public bool NeedsStep() => Close && !Finish;
    }
}
