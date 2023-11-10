using System;
using GGG.Classes.Tutorial;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialTutorial", menuName = "Game/Tutorials/InitialTutorial")]
public class InitialTutorial : TutorialBase
{
    public override void StartTutorial(Action OnTutorialEnd)
    {
        OnTutorialFinish += OnTutorialEnd;
    }
}
