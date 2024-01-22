using GGG.Components.UI;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class ExpeditionTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            LateralUI.OnLateralUiOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            LateralUI.OnLateralUiOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
