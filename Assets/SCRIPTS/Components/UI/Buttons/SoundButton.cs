using UnityEngine;
using UnityEngine.EventSystems;

namespace GGG.Components.UI.Buttons
{
    public class SoundButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.Play("Button");
        }
    }
}
