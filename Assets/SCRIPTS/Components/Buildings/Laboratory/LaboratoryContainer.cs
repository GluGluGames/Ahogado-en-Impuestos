using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryContainer : MonoBehaviour
    {
        private enum ContainerType
        {
            SeaResources,
            FishResources,
            ExpeditionResources,
            Buildings
        }

        [SerializeField] private ContainerType Type;
        [SerializeField] private int Index;

        private GameObject _viewport;

        private List<Resource> _resources = new ();
        private List<Building> _buildings = new ();
        private List<LaboratoryButton> _buttons = new();

        private void OnEnable()
        {
            if (_buttons.Count <= 0) _buttons = GetComponentsInChildren<LaboratoryButton>().ToList();
            if (Type != ContainerType.Buildings && _resources.Count <= 0)
                _resources = Resources.LoadAll<Resource>(Type.ToString()).ToList();
            else if (Type == ContainerType.Buildings && _buildings.Count <= 0)
                _buildings = Resources.LoadAll<Building>(Type.ToString()).ToList();
            
            InitializeButtons();

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        private void OnDisable()
        {
            _viewport.SetActive(false);
        }

        public List<LaboratoryButton> Buttons() => _buttons;

        private void InitializeButtons()
        {
            int count = _resources.Count > 0 ? _resources.Count : _buildings.Count;
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (count > i)
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
