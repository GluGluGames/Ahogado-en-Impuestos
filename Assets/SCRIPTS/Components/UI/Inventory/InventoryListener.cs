using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.UI.Buttons;
using UnityEngine;

namespace GGG.Components.UI.Inventory
{
    public class InventoryListener : MonoBehaviour
    {
        private List<ContainerButton> _containerButtons;
        private List<InventoryContainer> _containers;
        private ContainerButton _currentButton;
        private bool _initialized;

        private void Awake()
        {
            _containerButtons = GetComponentsInChildren<ContainerButton>().ToList();
            InventoryUI.OnInventoryOpen += Initialize;
            InventoryUI.OnInventoryClose += Deinitialize;
        }

        private void OnDisable()
        {
            InventoryUI.OnInventoryOpen -= Initialize;
            InventoryUI.OnInventoryClose -= Deinitialize;
        }

        private void Initialize()
        {
            InitializeContainerButtons();
        }

        private void Deinitialize()
        {
            _currentButton.DeselectButton();
            _currentButton = null;
            _containerButtons.ForEach(x => x.OnSelect -= OnContainerSelect);
        }

        private void InitializeContainerButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                if (button.GetIndex() == 0)
                {
                    _currentButton = button;
                    _currentButton.SelectButton();
                }

                button.OnSelect += OnContainerSelect;
            }
        }

        private void OnContainerSelect(ContainerButton button)
        {
            if (_currentButton) _currentButton.DeselectButton();

            _currentButton = button;
        }
    }
}
