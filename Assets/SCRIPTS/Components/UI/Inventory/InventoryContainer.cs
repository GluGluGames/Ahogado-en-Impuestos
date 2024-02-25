using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.UI.Inventory
{
    public class InventoryContainer : MonoBehaviour
    {
        [SerializeField] private int Index;
        [SerializeField] private ContainerType Type;
        
        private List<InventoryButton> _buttons;
        private List<InventoryResourceAmount> _resourceAmounts;
        private List<Resource> _resources;
        private GameObject _viewport;
        
        private enum ContainerType
        {
            SeaResources,
            FishResources,
            ExpeditionResources,
        }

        private void Awake()
        {
            _buttons = GetComponentsInChildren<InventoryButton>().ToList();
            _resourceAmounts = GetComponentsInChildren<InventoryResourceAmount>().ToList();
            _viewport = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            _resources = Resources.LoadAll<Resource>(Type.ToString()).ToList();
            InitializeButtons();
            
            _viewport.SetActive(Index == 0);
        }

        private void InitializeButtons()
        {
            int count = _resources.Count;
            
            for (int i = 0; i < _buttons.Count; i++)
            {
                if(count > i) 
                {
                    _buttons[i].InitializeButton(_resources[i]);
                    _resourceAmounts[i].Initialize(_resources[i]);
                    
                    continue;
                }
                 
                _buttons[i].Hide();
            }
        }
    }
}
