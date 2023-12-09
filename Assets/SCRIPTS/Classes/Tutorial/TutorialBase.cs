using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace GGG.Classes.Tutorial
{
    public abstract class TutorialBase : ScriptableObject
    {
        [SerializeField] protected string TutorialKey;
        [SerializeField] protected DialogueText[] Dialogues;
        [SerializeField] protected TutorialObjective[] Objectives;
        [SerializeField] protected TutorialPanel[] Panels;

        protected int _currentPanel;
        protected bool _nextStep;
        
        private void OnDisable()
        {
            _currentPanel = 0;
            _nextStep = false;
        }

        public void NextStep() => _nextStep = true;
        
        
        protected IEnumerator TutorialOpen(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange, bool tutorialEnd, bool closePanel, bool closeUi)
        {
            OnUiChange?.Invoke(Panels[_currentPanel].GetTitle(), 
                Panels[_currentPanel].GetImage(), 
                Panels[_currentPanel].GetText());
            _currentPanel++;
            OnTutorialStart?.Invoke();
        
            yield return new WaitUntil(() => _nextStep);
            if(closePanel) OnTutorialEnd?.Invoke(tutorialEnd, closeUi);
            _nextStep = false;
        }

        protected void ObjectivesPanelOpen(Action<TutorialObjective> OnObjectivesChange, int idx)
        {
            if (Objectives.Length <= 0)
                throw new Exception("No objectives set");

            if (idx == -1)
            {
                OnObjectivesChange?.Invoke(null);
                return;
            }
            
            OnObjectivesChange?.Invoke(Objectives[idx]);
        }

        public bool Completed() => PlayerPrefs.HasKey(TutorialKey) && PlayerPrefs.GetInt(TutorialKey) == 1;
        public string GetKey() => TutorialKey;

        public abstract IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange, Action<TutorialObjective> OnObjectivesChange);

        protected virtual void InitializeTutorial()
        {
            throw new NotImplementedException();
        }

        protected virtual void FinishTutorial()
        {
            PlayerPrefs.SetInt(TutorialKey, 1);
        }
    }
}
