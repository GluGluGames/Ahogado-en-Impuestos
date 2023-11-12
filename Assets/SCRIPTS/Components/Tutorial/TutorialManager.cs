using System;
using GGG.Components.Core;
using GGG.Classes.Tutorial;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        private GraphicRaycaster _raycaster;

        private void Start()
        {
            SceneManagement.Instance.OnGameSceneLoaded += () => StartTutorial("InitialTutorial");
            _ui = GetComponentInChildren<TutorialUI>();
            
            _raycaster = GetComponent<GraphicRaycaster>();
            _raycaster.enabled = false;
        }

        private void OnValidate()
        {
            Tutorials = Resources.LoadAll<TutorialBase>("Tutorials").ToList();
        }

        private void StartTutorial(string tutorialKey)
        {
            TutorialBase tutorial = Tutorials.Find((x) => x.GetKey() == tutorialKey);
            
            if (!tutorial)
                throw new Exception("Not tutorial found");

            if (!tutorial.Completed())
            {
                _raycaster.enabled = true;
                _ui.OnContinueButton += () => StartCoroutine(tutorial.NextStep());
                StartCoroutine(tutorial.StartTutorial(_ui.Open, _ui.Close, _ui.SetTutorialFields));
            }
        }
    }
}
