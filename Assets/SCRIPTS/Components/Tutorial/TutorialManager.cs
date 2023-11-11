using System;
using GGG.Components.Core;
using GGG.Classes.Tutorial;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        private void Start()
        {
            _ui = FindObjectOfType<TutorialUI>();
            
            GameManager.OnGameStart += () => StartTutorial("InitialTutorial");
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
                
            if(!tutorial.Completed()) tutorial.StartTutorial(_ui.Close);
        }
    }
}
