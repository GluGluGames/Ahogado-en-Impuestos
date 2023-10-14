using GGG.Components.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        private GameObject _panel;
        private BuildButton[] _buttons;

        private void Start()
        {
            _panel = transform.GetChild(0).gameObject;
            _panel.SetActive(false);

            _buttons = FindObjectsOfType<BuildButton>(true);

            foreach (BuildButton button in _buttons)
                button.OnStructureBuild += UpdateBuildings;
        }

        private void UpdateBuildings(BuildingComponent building)
        {
            building.OnBuildInteract += OnBuildInteract;
        }

        private void OnBuildInteract(Action action) 
        { 
            _panel.SetActive(true);
        }
    }
}
