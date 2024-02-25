using System;
using GGG.Components.Player;
using GGG.Shared;
using TMPro;
using UnityEngine;

namespace GGG.Components.UI.Inventory
{
    public class InventoryResourceAmount : MonoBehaviour
    {
        private TMP_Text _text;
        private Resource _resource;
        private bool _isOpen;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (!_isOpen) return;
            
            _text.SetText(PlayerManager.Instance.GetResourceCount(_resource.GetKey()).ToString());
        }

        public void Initialize(Resource resource)
        {
            _isOpen = true;
            _resource = resource;
        }

        private void Deinitialize()
        {
            _isOpen = false;
            _resource = null;
        }
    }
}
