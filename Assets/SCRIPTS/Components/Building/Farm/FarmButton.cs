using System;
using GGG.Shared;
using Project.Component.UI.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.Buildings
{
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public class FarmButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image Icon;

        private Image _border;
        private Resource _resource;
        private Tooltip _tooltip;
        private bool _selected;
        
        public Action<Resource, FarmButton> OnSelect;

        private void Awake()
        {
            _tooltip = GetComponent<Tooltip>();
            _border = GetComponent<Image>();
        }
        
        private void OnDisable()
        {
            _selected = false;
            _resource = null;
        }

        public void Initialize(Resource resource)
        {
            Show();
            _resource = resource;
            Icon.sprite = resource.GetSprite();

            Icon.color = resource.Unlocked() ? Color.white : Color.black;
            HideBorder();
            _tooltip.SetResourceName(resource.GetName());
        }

        public void OnResourceSelected()
        {
            if (_selected || !_resource.Unlocked()) return;
            
            Select();
            OnSelect?.Invoke(_resource, this);
        }

        public void Select()
        {
            Icon.sprite = _resource.GetSelectedSprite();
            ShowBorder();
            _selected = true;
        }

        public void Deselect()
        {
            Icon.sprite = _resource.GetSprite();
            HideBorder();
            _selected = false;
        }

        public void Hide() => gameObject.SetActive(false);
        private void Show() => gameObject.SetActive(true);
        public void ShowBorder() => _border.color = Color.white;
        private void HideBorder() => _border.color = new Color(1, 1, 1, 0);

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selected || !_resource) return;

            Icon.sprite = _resource.GetSelectedSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selected || !_resource) return;

            Icon.sprite = _resource.GetSprite();
        }
    }
}
