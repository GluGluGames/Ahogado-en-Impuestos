using System;
using System.Collections;
using GGG.Classes.Dialogue;
using UnityEngine;
using UnityEngine.Events;

namespace GGG.Classes.Tutorial
{
    public abstract class TutorialBase : ScriptableObject
    {
        [SerializeField] private string TutorialKey;
        [SerializeField] protected DialogueText[] Dialogues;
        [SerializeField] protected TutorialPanel[] Panels;
        [SerializeField] protected bool TutorialCompleted;

        protected int _currentPanel;
        protected bool _nextStep;
        
        private void OnDisable()
        {
            _currentPanel = 0;
            _nextStep = false;
        }

        public IEnumerator NextStep()
        {
            _nextStep = true;
            yield return null;
            _nextStep = false;
        }
        
        protected IEnumerator TutorialOpen(Action OnTutorialStart, Action OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            OnUiChange?.Invoke(Panels[_currentPanel].GetTitle(), 
                Panels[_currentPanel].GetImage(), 
                Panels[_currentPanel].GetText());
            _currentPanel++;
            OnTutorialStart?.Invoke();
        
            yield return new WaitUntil(() => _nextStep);
            OnTutorialEnd?.Invoke();
        }

        public bool Completed() => TutorialCompleted;
        public string GetKey() => TutorialKey;

        public abstract IEnumerator StartTutorial(Action OnTutorialStart, Action OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange);
    }
}
