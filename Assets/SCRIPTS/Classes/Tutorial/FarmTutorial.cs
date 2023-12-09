using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Tutorial;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "FarmTutorial", menuName = "Game/Tutorials/FarmTutorial")]
    public class FarmTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, Action<string, Sprite, string> OnUiChange, Action<TutorialObjective> OnObjectivesChange)
        {
            for (int i = 0; i < 2; i++)
                yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, false);
            
            FinishTutorial();
        }
    }
}
