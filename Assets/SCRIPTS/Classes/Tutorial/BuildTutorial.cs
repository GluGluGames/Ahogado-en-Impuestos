using System;
using System.Collections;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "BuildTutorial", menuName = "Game/Tutorials/BuildTutorial")]
    public class BuildTutorial : TutorialBase
    {
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true);
        }

        private IEnumerator BuildStep() {
            yield return null;
        }
    }
}
