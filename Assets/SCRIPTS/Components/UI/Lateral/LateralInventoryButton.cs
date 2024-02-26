using System;
using GGG.Components.Core;
using GGG.Components.UI.Inventory;
using UnityEngine;

namespace GGG.Components.UI.Lateral
{
    public class LateralInventoryButton : MonoBehaviour
    {
        public Action OnInventoryButton;

        private InventoryUI _inventory;

        private void Awake()
        {
            _inventory = FindObjectOfType<InventoryUI>();
        }

        public void OpenInventory()
        {
            if (GameManager.Instance.OnTutorial()) return;
            
            OnInventoryButton?.Invoke();
            _inventory.Open();
        }
    }
}
