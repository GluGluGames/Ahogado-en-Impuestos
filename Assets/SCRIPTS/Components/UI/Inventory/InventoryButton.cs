using System;
using GGG.Shared;
using Project.Component.UI.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.UI.Inventory
{
    public class InventoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image Icon;
        
        private Tooltip _tooltip;
        private Resource _resource;

        public Action OnSelect;

        private void Awake()
        {
            _tooltip = GetComponent<Tooltip>();
        }

        public void InitializeButton(Resource resource)
        {
            _resource = resource;
            Icon.sprite = resource.GetSprite();
            _tooltip.SetResourceName(resource.GetName());
        }
        
        public void OnResourceSelect()
        {
            if (_resource.GetKey().Equals("Seaweed")) return;
            
            if (HUDManager.Instance.ResourceBeingShown(_resource))
            {
                if(HUDManager.Instance.HideResource(_resource))
                    Icon.sprite = _resource.GetSprite();
                return;
            }
            
            if(HUDManager.Instance.ShowResource(_resource))
                Icon.sprite = _resource.GetSelectedSprite();
            
            OnSelect?.Invoke();
        }

        public void Hide() => transform.parent.gameObject.SetActive(false);
        public void OnPointerEnter(PointerEventData eventData)
        {
            Icon.sprite = _resource.GetSelectedSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Icon.sprite = _resource.GetSprite();
        }
    }
}
