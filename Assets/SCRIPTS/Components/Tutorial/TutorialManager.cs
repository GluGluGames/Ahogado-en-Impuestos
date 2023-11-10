using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Tutorial;
using UnityEngine;

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

        public void StartTutorial(TutorialBase tutorial)
        {
            if (tutorial.Started()) return;

            tutorial.StartTutorial(CloseUI);
        }

        private void CloseUI()
        {
            
        }
    }
}
