using GGG.Components.Buildings;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class FarmTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            FarmUI.OnFarmUIOpen += Aux;
        }

        protected override void FinishTutorial()
        {
            FarmUI.OnFarmUIOpen -= Aux;
            base.FinishTutorial();
        }

        private void Aux(Farm farm) => StartTutorialNoEnum();
    }
}
