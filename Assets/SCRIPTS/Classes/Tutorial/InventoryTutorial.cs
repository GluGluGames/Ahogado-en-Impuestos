using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "InventoryTutorial", menuName = "Game/Tutorials/InventoryTutorial")]
    public class InventoryTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd,
            Action<string, Sprite, string> OnUiChange)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, false);
            FinishTutorial();
        }

        protected override void InitializeTutorial()
        {
            throw new NotImplementedException();
        }

        protected override void FinishTutorial()
        {
            TutorialCompleted = true;
        }
    }
}
