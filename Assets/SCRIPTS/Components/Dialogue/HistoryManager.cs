using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Dialogue;
using GGG.Components.Core;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Dialogue
{
    public class HistoryManager : MonoBehaviour
    {
        public static HistoryManager Instance;

        private void Awake()
        {
            if (Instance != null) return;

            Instance = this;
        }

        [SerializeField] private DialogueText Dialogue;

        private const string _HISTORY_KEY = "History";
        private DialogueBox _dialogueBox;

        public Action OnHistoryEnd;

        private IEnumerator Start()
        {
            yield return null;
            
            if (PlayerPrefs.HasKey(_HISTORY_KEY) && PlayerPrefs.GetInt(_HISTORY_KEY) == 1)
            {
                OnHistoryEnd?.Invoke();
                yield break;
            }
            
            _dialogueBox = FindObjectOfType<DialogueBox>();
            _dialogueBox.AddNewDialogue(Dialogue);

            _dialogueBox.DialogueStart += StartHistory;
            _dialogueBox.DialogueEnd += EndHistory;
        }

        private void StartHistory()
        {
            GameManager.Instance.SetCurrentTutorial(Tutorials.InitialTutorial);
        }

        private void EndHistory()
        {
            PlayerPrefs.SetInt(_HISTORY_KEY, 1);
            OnHistoryEnd?.Invoke();
            _dialogueBox.DialogueEnd -= EndHistory;
            _dialogueBox.DialogueStart -= StartHistory;
        }
    }
}
