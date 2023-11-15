using System;
using GGG.Components.Core;
using GGG.Classes.Tutorial;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GGG.Components.Buildings;
using GGG.Components.UI;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance;

        private void Awake()
        {
            if (Instance != null) return;

            Instance = this;
        }

        [SerializeField] private List<TutorialBase> Tutorials;

        private TutorialUI _ui;
        private GameManager _gameManager;
        private GraphicRaycaster _raycaster;

        private void Start()
        {
            SceneManagement.Instance.OnGameSceneLoaded += InitializeTutorials;
            _gameManager = GameManager.Instance;
            _ui = GetComponentInChildren<TutorialUI>();
            
            _raycaster = GetComponent<GraphicRaycaster>();
            _raycaster.enabled = false;
        }

        private void OnValidate()
        {
            Tutorials = Resources.LoadAll<TutorialBase>("Tutorials").ToList();
        }

        private void InitializeTutorials()
        {
            StartTutorial("InitialTutorial", null);
            BuildingUI.OnUiOpen += () => StartTutorial("BuildTutorial", "InitialTutorial");
        }

        private void StartTutorial(string tutorialKey, string previousTutorialKey)
        {
            if (_gameManager.GetCurrentTutorial() != Shared.Tutorials.None) return;
            
            TutorialBase tutorial = Tutorials.Find((x) => x.GetKey() == tutorialKey);
            
            if (!tutorial)
                throw new Exception("Not tutorial found");
            
            if (tutorial.Completed()) return;
            
            if (!string.IsNullOrEmpty(previousTutorialKey))
            {
                TutorialBase previousTutorial = Tutorials.Find(x => x.GetKey() == previousTutorialKey);
                if (!previousTutorial) throw new Exception("Previous tutorial not found");
                if (!previousTutorial.Completed()) return;
            }
            
            _raycaster.enabled = true;
            _ui.OnContinueButton += tutorial.NextStep;
            if (!Enum.TryParse(tutorialKey, out Tutorials tutorialState))
                throw new Exception("Enum or key not correct");
            
            _gameManager.SetCurrentTutorial(tutorialState);
            StartCoroutine(tutorial.StartTutorial(_ui.Open, _ui.Close, _ui.SetTutorialFields));
        }
    }
}
