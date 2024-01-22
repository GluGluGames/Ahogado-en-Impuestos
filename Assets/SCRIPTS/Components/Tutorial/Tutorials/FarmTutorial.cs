using GGG.Components.Buildings;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class FarmTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            FarmUI.OnFarmUIOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            FarmUI.OnFarmUIOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
