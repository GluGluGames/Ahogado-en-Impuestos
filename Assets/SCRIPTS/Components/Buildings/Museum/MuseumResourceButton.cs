using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using Project.Component.UI.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Museum
{
    [RequireComponent(typeof(Button))]
    public class MuseumResourceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image Icon;

        private Tooltip _tooltip;
        private Resource _resource;
        private Building _building;

        private bool _selected;

        public Action<Resource, Building> OnSelect;
        public Action<MuseumResourceButton> OnClick;

        private void OnEnable()
        {
            if (!_tooltip) _tooltip = GetComponent<Tooltip>();
        }

        public void InitializeButton(Resource resource)
        {
            _resource = resource;
            Icon.color = resource.Unlocked() ? Color.white : Color.black;
            
            Icon.sprite = resource.GetSprite();
            _tooltip.SetResourceName(resource.GetName());
        }

        public void InitializeButton(Building building)
        {
            _building = building;
            Icon.color = building.IsUnlocked() ? Color.white : Color.black;
            
            Icon.sprite = building.GetResearchIcon();
            _tooltip.SetResourceName(building.GetName());
        }

        public void OnButtonSelect()
        {
            if (_selected) return;
            
            bool unlocked = _resource ? _resource.Unlocked() : _building.IsUnlocked();
            if (!unlocked)
            {
                OnSelect?.Invoke(_resource, _building);
                return;
            }
            
            Icon.sprite = _building ? _building.GetSelectedIcon() : _resource.GetSelectedSprite();
            _selected = true;
            
            OnSelect?.Invoke(_resource, _building);
            OnClick?.Invoke(this);
        }

        public void OnButtonDeselect()
        {
            Icon.sprite = _building ? _building.GetResearchIcon() : _resource.GetSprite();
            _selected = false;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Icon.sprite = _building ? _building.GetSelectedIcon() : _resource.GetSelectedSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selected) return;
            
            Icon.sprite = _building ? _building.GetResearchIcon() : _resource.GetSprite();
        }
    }
}
