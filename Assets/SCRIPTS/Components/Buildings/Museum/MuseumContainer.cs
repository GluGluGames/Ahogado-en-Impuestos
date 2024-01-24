using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Components.UI.Buttons;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Museum
{
    public class MuseumContainer : MonoBehaviour
    {
        [SerializeField] private ContainerType Type;
        [SerializeField, Range(0, 3)] private int Index;

        private enum ContainerType
        {
            SeaResources,
            FishResources,
            ExpeditionResources,
            Buildings
        }

        private GameObject _viewport;

        private List<Resource> _resources = new ();
        private List<Building> _buildings = new ();
        private List<MuseumResourceButton> _buttons = new ();

        public List<MuseumResourceButton> Buttons() => _buttons;

        private void OnEnable()
        {
            if (_buttons.Count <= 0) _buttons = GetComponentsInChildren<MuseumResourceButton>().ToList();

            if (Type != ContainerType.Buildings && _resources.Count <= 0)
                _resources = Resources.LoadAll<Resource>(Type.ToString()).ToList();
            else if (Type == ContainerType.Buildings && _buildings.Count <= 0)
                _buildings = Resources.LoadAll<Building>(Type.ToString()).ToList();
            
            InitializeButtons();

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(Index == 0);
        }

        private void OnDisable()
        {
            _viewport.SetActive(Index == 0);
        }

        private void InitializeButtons()
        {
            int count = _resources.Count > 0 ? _resources.Count : _buildings.Count;
            bool found = false;
            
            for (int i = 0; i < _buttons.Count; i++)
            {
                 if(count > i) 
                 {
                    if (_resources.Count > 0) _buttons[i].InitializeButton(_resources[i]);
                    else _buttons[i].InitializeButton(_buildings[i]);
                    
                    continue;
                 }
                 
                 _buttons[i].Hide();
            }
        }
    }
}
