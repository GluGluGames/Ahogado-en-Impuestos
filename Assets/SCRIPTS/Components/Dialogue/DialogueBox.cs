using System;
using GGG.Input;

using System.Collections.Generic;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.Dialogue
{
    public class DialogueBox : MonoBehaviour {
        [Header("Dialogue")]
        [SerializeField] [TextArea] private List<string> DialogueLines;
        [SerializeField] private float TypingSpeed = 0.1f;

        [Space(5)] 
        [Header("Other Fields")] 
        [SerializeField] private List<Sprite> AvatarList;
        [SerializeField] private Image Avatar;
        [SerializeField] private List<string> CharacterNames;
        [SerializeField] private TMP_Text NameText;

        private InputManager _input;
        private TMP_Text _text;
        private CanvasGroup _canvasGroup;
        
        private int _lineIdx;
        private int _avatarIdx;
        private int _characterIdx;
        private bool _started;
        private bool _finished;
        private bool _currentTextFinished;
        private StringBuilder _currentText;

        public Action DialogueStart;
        public Action DialogueEnd;

        private void Start() {
            _input = InputManager.Instance;
            _text = GetComponent<TMP_Text>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _started = false;
            _finished = false;
            _currentTextFinished = false;
            
            _canvasGroup.alpha = 0f;
            _lineIdx = 0;
            _avatarIdx = 0;
            _characterIdx = 0;
            
            NameText.SetText(CharacterNames[_characterIdx]);
            Avatar.sprite = AvatarList[_avatarIdx];
        }

        private void Update() {
            if (!_started || _finished) return;
            
            OnDialogueContinue();
        }

        private void OnDialogueStart() {
            if (_finished || _started) return;
            
            StartCoroutine(TypeText(DialogueLines[_lineIdx++]));
            DialogueStart.Invoke();
            _canvasGroup.alpha = 1f;
            _started = true;
        }

        private void OnDialogueContinue() {
            if (!_input.MouseClick()) return;
            
            if (_lineIdx < DialogueLines.Count) {
                if (!_currentTextFinished) {
                    ShowAllText();
                    return;
                }

                StartCoroutine(TypeText(DialogueLines[_lineIdx++]));
                if(_avatarIdx < AvatarList.Count)
                    Avatar.sprite = AvatarList[_avatarIdx++];
                if(_characterIdx < CharacterNames.Count)
                    NameText.SetText(CharacterNames[_characterIdx++]);
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
            _canvasGroup.alpha = 0f;
            _finished = true;
            DialogueEnd?.Invoke();
        }

        private void OnValidate() {
            if (DialogueLines.Count <= 0) return;

            if (_text == null) _text = GetComponent<TMP_Text>();
            _text.SetText(DialogueLines[0]);
        }

        private IEnumerator TypeText(string text) {
            _currentText = new StringBuilder();
            _currentTextFinished = false;
            
            for (int i = 0; i < text.Length; i++) {
                _currentText.Append(text[i]);
                _text.SetText(_currentText.ToString());
                yield return new WaitForSeconds(TypingSpeed);
            }
            
            _currentTextFinished = true;
        }

        private void ShowAllText() {
            StopAllCoroutines();
            _text.SetText(DialogueLines[_lineIdx - 1]);
            _currentTextFinished = true;
        }
    }
}
