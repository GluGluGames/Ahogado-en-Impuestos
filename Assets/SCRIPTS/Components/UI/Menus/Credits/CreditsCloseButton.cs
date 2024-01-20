using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class CreditsCloseButton : MonoBehaviour
    {
        public Action OnClose;

        public void OnCloseButton()
        {
            OnClose?.Invoke();
        }
    }
}
