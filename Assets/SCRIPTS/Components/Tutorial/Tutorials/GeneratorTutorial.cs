using GGG.Components.Buildings.Generator;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class GeneratorTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            GeneratorUI.OnGeneratorOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            GeneratorUI.OnGeneratorOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
