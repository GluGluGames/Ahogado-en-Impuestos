using GGG.Input;
using GGG.Classes.Dialogue;

using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.Dialogue
{
    public class DialogueBox : MonoBehaviour {
        [Header("Dialogue")]
        [SerializeField] private float TypingSpeed = 0.1f;
        [SerializeField] private TMP_Text DialogueText;
        [SerializeField] private Sound DialogueSound;
        [SerializeField] private Image DialogueColor;

        [Space(5)] 
        [Header("Other Fields")] 
        [SerializeField] private Image Avatar;
        [SerializeField] private TMP_Text NameText;

        private InputManager _input;
        private SoundManager _sound;

        private const float _DIALOGUE_THRESHOLD = 1f;
        
        private DialogueText _currentDialogue;
        private bool _currentTextFinished;
        private bool _started;
        private float _delta;
        private StringBuilder _currentText;

        public Action DialogueStart;
        public Action DialogueEnd;

        private void Start() {
            _input = InputManager.Instance;
            _sound = SoundManager.Instance;
            _delta = _DIALOGUE_THRESHOLD;
        }

        private void Update() {
            if (_currentDialogue == null) return;
            
            OnDialogueContinue();
        }

        public void StartDialogue() {
            if (_started) return;
            
            DialogueStart.Invoke();
            
            Avatar.sprite = _currentDialogue.GetNextAvatar();
            NameText.SetText(_currentDialogue.GetNextName());
            DialogueColor.sprite = _currentDialogue.GetBoxColor();
            
            StartCoroutine(TypeText(_currentDialogue.GetNextDialogue()));
            _started = true;
        }

        private void OnDialogueContinue() {
            if (_delta > 0)
            {
                _delta -= Time.deltaTime;
                return;
            }
            
            if (!_input.MouseClick()) return;
            
            if (!_currentDialogue.DialogueEnd()) {
                if (!_currentTextFinished) {
                    ShowAllText();
                    return;
                }

                if(_currentDialogue.IsMoreAvatars()) Avatar.sprite = _currentDialogue.GetNextAvatar();
                if(_currentDialogue.IsMoreNames()) NameText.SetText(_currentDialogue.GetNextName());
                
                _delta = _DIALOGUE_THRESHOLD;
                StartCoroutine(TypeText(_currentDialogue.GetNextDialogue()));
            }
            else {
                if (!_currentTextFinished) {
                    ShowAllText();
                    return;
                }
                
                OnDialogueEnd();
            }
        }

        private void OnDialogueEnd() {
            _started = false;
            _delta = _DIALOGUE_THRESHOLD;
            _currentDialogue.ResetDialogue();
            _currentDialogue = null;
            DialogueEnd?.Invoke();
        }

        public void AddNewDialogue(DialogueText dialogue)
        {
            _currentDialogue = dialogue;
            StartDialogue();
        }

        private IEnumerator TypeText(string text) {
            _currentText = new StringBuilder();
            _currentTextFinished = false;
            
            for (int i = 0; i < text.Length; i++) {
                _currentText.Append(text[i]);
                DialogueText.SetText(_currentText.ToString());
                _sound.Play(DialogueSound);
                yield return new WaitForSeconds(TypingSpeed);
            }
            
            _currentTextFinished = true;
        }

        private void ShowAllText() {
            StopAllCoroutines();
            DialogueText.SetText(_currentDialogue.GetPreviousDialogue());
            _currentTextFinished = true;
        }
    }
}
