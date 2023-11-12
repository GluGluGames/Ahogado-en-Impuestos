using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "UpgradeTutorial", menuName = "Game/Tutorials/UpgradeTutorial")]
    public class UpgradeTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            throw new System.NotImplementedException();
        }
    }
}
