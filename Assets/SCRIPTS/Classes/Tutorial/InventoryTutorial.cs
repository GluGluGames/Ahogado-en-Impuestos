using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "InventoryTutorial", menuName = "Game/Tutorials/InventoryTutorial")]
    public class InventoryTutorial : TutorialBase
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
