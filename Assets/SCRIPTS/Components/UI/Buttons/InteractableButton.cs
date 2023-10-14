using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GGG.Components.UI {
    public class InteractableButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(Vector3.one, 1f);
        }

        public virtual void OnPointerDown(PointerEventData eventData) { print($"TODO - Implement button on {gameObject.name}"); }
    }
}
