using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "GeneratorTutorial", menuName = "Game/Tutorials/GeneratorTutorial")]
    public class GeneratorTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd,
            Action<string, Sprite, string> OnUiChange, Action<TutorialObjective> OnObjectivesChange)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, false);
            
            FinishTutorial();
        }
    }
}
