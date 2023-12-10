using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "ExpeditionTutorial", menuName = "Game/Tutorials/ExpeditionTutorial")]
    public class ExpeditionTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, Action<string, Sprite, string> OnUiChange, Action<TutorialObjective> OnObjectivesChange)
        {
            for (int i = 0; i < 3; i++)
                yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, false);
        }
    }
}
