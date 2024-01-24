using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.UI.Buttons;
using UnityEngine;

namespace GGG.Components.Buildings.Museum
{
    public class MuseumListener : MonoBehaviour
    {
        private MuseumDescription _description;
        private List<MuseumContainer> _containers = new ();
        private List<ContainerButton> _containerButtons = new ();

        private MuseumResourceButton _selectedButton;
        private ContainerButton _containerButton;
        private bool _initialized;
        
        private void OnEnable()
        {
            if (!_description) _description = GetComponentInChildren<MuseumDescription>(true);
            
            if (_containers.Count <= 0) _containers = GetComponentsInChildren<MuseumContainer>(true).ToList();
            if (_containerButtons.Count <= 0)
                _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();

            MuseumUI.OnMuseumOpen += InitializeContainerButtons;
            MuseumUI.OnMuseumOpen += InitializeButtons;
            
            MuseumUI.OnMuseumClose += DeinitializeContainerButtons;
        }

        private void OnDisable()
        {
            DeinitializeButtons();
            
            MuseumUI.OnMuseumOpen -= InitializeButtons;
            MuseumUI.OnMuseumOpen -= InitializeContainerButtons;
            MuseumUI.OnMuseumClose -= DeinitializeContainerButtons;
            
            _initialized = false;
        }

        private void OnContainerSelect(ContainerButton button)
        {
            if (_containerButton) _containerButton.DeselectButton();
            _containerButton = button;
        }

        private void OnButtonSelect(MuseumResourceButton button)
        {
            if (_selectedButton) _selectedButton.OnButtonDeselect();
            _selectedButton = button;
        }

        private void InitializeContainerButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                if (button.GetIndex() == 0)
                {
                    _containerButton = button;
                    _containerButton.SelectButton();
                }
                button.OnSelect += OnContainerSelect;
            }
        }
        
        private void DeinitializeContainerButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                button.DeselectButton();
                button.OnSelect -= OnContainerSelect;
            }
            
            _containerButton = null;
            _selectedButton.OnButtonDeselect();
            _selectedButton = null;
        }

        private void InitializeButtons()
        {
            if (_initialized) return;

            foreach (MuseumContainer container in _containers)
            {
                foreach (MuseumResourceButton button in container.Buttons())
                {
                    button.OnSelect += _description.SetText;
                    button.OnClick += OnButtonSelect;
                }
            }

            _initialized = true;
        }
        
        private void DeinitializeButtons()
        {
            foreach (MuseumContainer container in _containers)
            {
                foreach (MuseumResourceButton button in container.Buttons())
                {
                    button.OnSelect -= _description.SetText;
                    button.OnClick -= OnButtonSelect;
                }
            }
        }
    }
}
