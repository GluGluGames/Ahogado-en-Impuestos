using System;
using GGG.Classes.Dialogue;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    public abstract class TutorialBase : ScriptableObject
    {
        [SerializeField] private DialogueText[] Dialogues;

        public static Action OnTutorialFinish;
        public static Action OnTutorialStart;

        public abstract void StartTutorial();
    }
}
