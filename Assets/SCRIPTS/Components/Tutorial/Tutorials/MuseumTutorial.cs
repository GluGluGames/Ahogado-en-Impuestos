using GGG.Components.Buildings.Museum;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class MuseumTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            MuseumUI.OnMuseumOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            MuseumUI.OnMuseumOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
