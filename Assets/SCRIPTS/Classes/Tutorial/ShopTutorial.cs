using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "ShopTutorial", menuName = "Game/Tutorials/ShopTutorial")]
    public class ShopTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            InitializeTutorial();
            yield return null;
            FinishTutorial();
        }

        protected override void InitializeTutorial()
        {
            throw new NotImplementedException();
        }

        protected override void FinishTutorial()
        {
            throw new NotImplementedException();
        }
    }
}
