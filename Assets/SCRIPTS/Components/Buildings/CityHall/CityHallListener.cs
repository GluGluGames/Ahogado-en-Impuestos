using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.UI.Buttons;
using UnityEngine;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallListener : MonoBehaviour
    {
        private List<ContainerButton> _containerButtons = new ();
        private ContainerButton _selectedButton;

        private void OnEnable()
        {
            if (_containerButtons.Count <= 0) 
                _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();
            
            CityHallUi.OnCityHallOpen += InitializeButtons;
            CityHallUi.OnCityHallClose += DeinitializeButtons;
        }

        private void OnDisable()
        {
            CityHallUi.OnCityHallOpen -= InitializeButtons;
            CityHallUi.OnCityHallClose -= DeinitializeButtons;
        }

        private void InitializeButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                if (button.GetIndex() == 0)
                {
                    _selectedButton = button;
                    _selectedButton.SelectButton();
                } else button.DeselectButton();

                button.OnSelect += OnContainerSelect;
            }
        }

        private void DeinitializeButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                button.DeselectButton();
                button.OnSelect -= OnContainerSelect;
            }

            _selectedButton = null;
        }

        private void OnContainerSelect(ContainerButton button)
        {
            if (_selectedButton) _selectedButton.DeselectButton();
            _selectedButton = button;
        }
    }
}
