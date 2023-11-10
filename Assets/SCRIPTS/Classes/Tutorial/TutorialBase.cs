using System;
using GGG.Classes.Dialogue;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    public abstract class TutorialBase : ScriptableObject
    {
        [SerializeField] private DialogueText[] Dialogues;
        [SerializeField] private bool Triggered;

        public static Action OnTutorialFinish;
        public static Action OnTutorialStart;

        public bool Started() => Triggered;

        public abstract void StartTutorial(Action OnTutorialEnd);
    }
}
