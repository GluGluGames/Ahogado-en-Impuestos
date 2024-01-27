using GGG.Components.Buildings.Laboratory;
using GGG.Components.Buildings.Museum;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class LaboratoryTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            LaboratoryUI.OnLaboratoryOpen += Aux;
        }

        protected override void FinishTutorial()
        {
            LaboratoryUI.OnLaboratoryOpen -= Aux;
            base.FinishTutorial();
        }

        private void Aux(Laboratory aux) => StartTutorialNoEnum();
    }
}
