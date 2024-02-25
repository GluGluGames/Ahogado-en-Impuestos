using System;
using UnityEngine;

namespace GGG.Components.UI.Inventory
{
    public class InventoryCloseButton : MonoBehaviour
    {
        public Action OnCloseButton;

        public void OnClose() => OnCloseButton?.Invoke();
    }
}
