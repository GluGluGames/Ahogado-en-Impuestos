using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "BuildTutorial", menuName = "Game/Tutorials/BuildTutorial")]
    public class BuildTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            throw new System.NotImplementedException();
        }
    }
}
