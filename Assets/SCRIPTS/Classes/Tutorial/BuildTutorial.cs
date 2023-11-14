using System;
using System.Collections;
using GGG.Components.UI;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "BuildTutorial", menuName = "Game/Tutorials/BuildTutorial")]
    public class BuildTutorial : TutorialBase
    {
        private bool _structureBuild;
        
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            _structureBuild = false;
            
            BuildButton[] buildButtons = FindObjectsOfType<BuildButton>();
            BuildingUI ui = FindObjectOfType<BuildingUI>();
            foreach (BuildButton button in buildButtons)
                button.StructureBuild += CheckStructureBuild;
            
            for (int i = 0; i < 2; i++)
            {
                yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false);
            }

            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true);
            yield return BuildStep();
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true);
            TutorialCompleted = true;
            ui.Close();
            
            foreach (BuildButton button in buildButtons)
                button.StructureBuild -= CheckStructureBuild;
        }

        private IEnumerator BuildStep() {
            while (!_structureBuild)
            {
                yield return null;
            }
        }
        
        private void CheckStructureBuild() => _structureBuild = true;
    }
}
