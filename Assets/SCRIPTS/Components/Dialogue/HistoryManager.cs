using System;
using System.Collections.Generic;
using GGG.Classes.Dialogue;
using GGG.Components.Core;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Dialogue
{
    public class HistoryManager : MonoBehaviour
    {
        [SerializeField] private DialogueText Dialogue;

        private const string _HISTORY_KEY = "History";
        private DialogueBox _dialogueBox;

        public static Action OnHistoryEnd;
        
        private void Start()
        {
            if (PlayerPrefs.HasKey(_HISTORY_KEY) && PlayerPrefs.GetInt(_HISTORY_KEY) == 1) return;
            
            _dialogueBox = FindObjectOfType<DialogueBox>();
            _dialogueBox.AddNewDialogue(Dialogue);
            _dialogueBox.DialogueEnd += EndHistory;
            GameManager.Instance.SetCurrentTutorial(Tutorials.InitialTutorial);
        }

        private void EndHistory()
        {
            GameManager.Instance.SetCurrentTutorial(Tutorials.None);
            PlayerPrefs.SetInt(_HISTORY_KEY, 1);
            OnHistoryEnd?.Invoke();
            _dialogueBox.DialogueEnd -= EndHistory;
        }
    }
}
