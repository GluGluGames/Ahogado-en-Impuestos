using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Components.Core;
using GGG.Components.Scenes;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Tutorial
{
    public abstract class Tutorial : MonoBehaviour
    {
        [SerializeField] private Tutorials Key;
        [SerializeField] protected List<TutorialPanel> Panels;

        private GameManager _gameManager;
        
        private TutorialUI _ui;
        private TutorialObjectives _objectives;
        private TutorialContinueButton _continue;
        
        protected List<IEnumerator> _steps;

        private bool _next;
        private int _currentPanel;
        private int _currentStep;

        private void Start()
        {
            TutorialCondition();
        }

        public TutorialPanel CurrentPanel() => Panels[_currentPanel];

        public bool Completed() => PlayerPrefs.HasKey(Key.ToString()) && PlayerPrefs.GetInt(Key.ToString()) == 1;

        public void Restore() => PlayerPrefs.SetInt(Key.ToString(), 0);

        protected void StartTutorialNoEnum() => StartCoroutine(StartTutorial());
        
        protected IEnumerator StartTutorial()
        {
            if (Completed()) yield break;
            
            InitializeTutorial();
            StartCoroutine(PlayTutorial());
        }

        private IEnumerator PlayTutorial()
        {
            WaitWhile wait = new(() => !_next);
            
            foreach (TutorialPanel panel in Panels)
            {
                OnPanelStart(panel);
                yield return wait;
                
                if (panel.NeedsStep())
                {
                    OnStep(panel);
                    yield return TutorialStep();
                }
                OnPanelFinish();
            }
            
            FinishTutorial();
        }

        private void NextStep() => _next = true;

        private void OnStep(TutorialPanel panel)
        {
            _ui.Close();
            _objectives.SetObjectives(panel.GetObjectives());
            _objectives.Show();
        }

        private void OnPanelStart(TutorialPanel panel)
        {
            _ui.FillUi(panel);
            if(_ui.Closed()) _ui.Open();
        }

        private void OnPanelFinish()
        {
            _objectives.Hide();
            _next = false;
            _currentPanel++;
        }

        private IEnumerator TutorialStep()
        {
            yield return _steps[_currentStep];
            _currentStep++;
        }
        protected abstract void TutorialCondition();

        protected virtual void FinishTutorial()
        {
            _continue.OnContinue -= NextStep;
            _ui.Close();
            PlayerPrefs.SetInt(Key.ToString(), 1);
            _gameManager.SetCurrentTutorial(Tutorials.None);
        }

        protected virtual void InitializeTutorial()
        {
            _gameManager = GameManager.Instance;
            _gameManager.SetCurrentTutorial(Key);
            
            _ui = FindObjectOfType<TutorialUI>();
            _ui.FillUi(CurrentPanel());
            _ui.Open();

            _objectives = FindObjectOfType<TutorialObjectives>();
            
            _continue = FindObjectOfType<TutorialContinueButton>(true);
            _continue.OnContinue += NextStep;
        }
    }
}
