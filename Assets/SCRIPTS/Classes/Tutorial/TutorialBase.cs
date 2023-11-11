using System;
using GGG.Classes.Dialogue;
using UnityEngine;
using UnityEngine.Events;

namespace GGG.Classes.Tutorial
{
    public abstract class TutorialBase : ScriptableObject
    {
        [SerializeField] private string TutorialKey;
        [SerializeField] private DialogueText[] Dialogues;
        [SerializeField] private TutorialPanel[] Panels;
        [SerializeField] private bool TutorialCompleted;

        public static Action OnTutorialFinish;
        public static Action OnTutorialStart;

        public bool Completed() => TutorialCompleted;
        public string GetKey() => TutorialKey;

        public abstract void StartTutorial(Action OnTutorialEnd);
    }
}
