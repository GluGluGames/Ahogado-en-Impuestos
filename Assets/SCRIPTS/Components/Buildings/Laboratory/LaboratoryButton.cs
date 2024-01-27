using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using Project.Component.UI.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    [RequireComponent(typeof(Button))]
    public class LaboratoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image Icon;
        
        private Tooltip _tooltip;
        private Resource _resource;
        private Building _building;

        public Action<Resource, Building> OnSelect;

        private void OnEnable()
        {
            if (!_tooltip) _tooltip = GetComponent<Tooltip>();
        }

        public void InitializeButton(Resource resource)
        {
            if (resource.BeingResearch() || resource.Unlocked())
            {
                Hide();
                return;
            }
            
            _resource = resource;
            Icon.color = resource.CanResearch() ? Color.white : Color.black;

            Icon.sprite = resource.GetSprite();
            _tooltip.SetResourceName(resource.GetName());
        }

        public void InitializeButton(Building building)
        {
            if (building.IsUnlocked() || building.BeingResearch())
            {
                Hide();
                return;
            }
            
            _building = building;

            Icon.sprite = building.GetResearchIcon();
            _tooltip.SetResourceName(building.GetName());
        }

        public void OnButtonSelect()
        {
            if (_resource && !_resource.CanResearch()) return;
            
            OnSelect?.Invoke(_resource, _building);
        }
        
        public void Hide() => gameObject.SetActive(false);
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Icon.sprite = _building ? _building.GetSelectedIcon() : _resource.GetSelectedSprite();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            Icon.sprite = _building ? _building.GetResearchIcon() : _resource.GetSprite();
        }
    }
}
