using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "ShopTutorial", menuName = "Game/Tutorials/ShopTutorial")]
    public class ShopTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            throw new System.NotImplementedException();
        }
    }
}
