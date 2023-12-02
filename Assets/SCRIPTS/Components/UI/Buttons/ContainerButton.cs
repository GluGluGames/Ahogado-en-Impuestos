using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.UI.Buttons
{
    public class ContainerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Images")] [SerializeField] private Image Icon;
        [SerializeField] private Sprite NormalIcon;
        [SerializeField] private Sprite SelectedIcon;

        [Space(5), Header("Panels")] [SerializeField]
        private GameObject ActivatePanel;
        
        private bool _selected;

        public Action OnButtonClick;

        public void Initialize()
        {
            _selected = ActivatePanel.activeInHierarchy;
            Icon.sprite = _selected ? SelectedIcon : NormalIcon;
        }

        public void DeselectButton()
        {
            ActivatePanel.SetActive(false);

            Icon.sprite = NormalIcon;
            _selected = false;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_selected) return;
            ActivatePanel.SetActive(true);

            Icon.sprite = SelectedIcon;
            _selected = true;
            OnButtonClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Icon.sprite = SelectedIcon;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selected) return;
            
            Icon.sprite = NormalIcon;
        }
    }
}
