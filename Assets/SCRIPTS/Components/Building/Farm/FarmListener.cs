using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class FarmListener : MonoBehaviour
    {
        private List<FarmButton> _buttons;
        private List<Resource> _resources;
        private FarmButton _selectedButton;
        private Farm _farm;

        public static Action<Resource> OnResourceSelected;

        private void Awake()
        {
            _buttons = GetComponentsInChildren<FarmButton>(true).ToList();
        }

        private void OnEnable()
        {
            FarmUI.OnFarmUIOpen += OnOpen;
            FarmUI.OnFarmClose += OnClose;
        }

        private void OnDisable()
        {
            FarmUI.OnFarmUIOpen -= OnOpen;
            FarmUI.OnFarmClose -= OnClose;
        }

        private void OnOpen(Farm farm)
        {
            _farm = farm;
            _resources = farm.Type() == FarmTypes.SeaFarm 
                ? Resources.LoadAll<Resource>("SeaResources").ToList() 
                : Resources.LoadAll<Resource>("FishResources").ToList();

            int a = _buttons.Count / 2 - 1;
            
            for (int i = 0; i < _buttons.Count; i++)
            {
                int idx = i % 2 == 0 ? i / 2 : i + a--;
                
                if(_resources.Count <= i) _buttons[idx].Hide();
                else
                {
                    _buttons[idx].Initialize(_resources[i]);
                    
                    if (farm.GetResource() && farm.GetResource() == _resources[i])
                    {
                        _buttons[idx].Select();
                        _selectedButton = _buttons[idx];
                    }

                    _buttons[idx].OnSelect += OnSelected;
                }
            }
        }

        private void OnClose()
        {
            _resources.Clear();
            _farm = null;
            
            _buttons.ForEach(x => x.OnSelect -= OnSelected);
        }

        private void OnSelected(Resource resource, FarmButton button)
        {
            if(_selectedButton) _selectedButton.Deselect();
            _selectedButton = button;
            
            _farm.Resource(resource);
            _farm.SetResourceModel(resource);
            
            OnResourceSelected?.Invoke(resource);
        }
    }
}