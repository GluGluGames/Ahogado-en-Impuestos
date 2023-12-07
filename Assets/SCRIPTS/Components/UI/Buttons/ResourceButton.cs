using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Buttons
{
    public class ResourceButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text ResourceAmount;
        [SerializeField] private Image Icon;

        public void SetResourceAmount(int amount) => ResourceAmount.SetText(amount.ToString());
        public void SetIcon(Sprite icon) => Icon.sprite = icon;
    }
}
