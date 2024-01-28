using GGG.Components.Buildings.Generator;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class GeneratorTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            GeneratorUI.OnGeneratorOpen += Aux;
        }

        protected override void FinishTutorial()
        {
            GeneratorUI.OnGeneratorOpen -= Aux;
            base.FinishTutorial();
        }

        private void Aux(Generator generator) => StartTutorialNoEnum();
    }
}
