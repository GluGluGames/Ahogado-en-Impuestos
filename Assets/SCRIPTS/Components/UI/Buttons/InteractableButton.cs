using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GGG.Components.UI {
    public class InteractableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(Vector3.one, 1f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.Play("Button");
        }

        private void OnDisable() {
            transform.DOKill();
        }
    }
}
