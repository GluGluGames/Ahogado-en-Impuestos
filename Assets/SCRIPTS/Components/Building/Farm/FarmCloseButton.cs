using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings
{
    [RequireComponent(typeof(Button))]
    public class FarmCloseButton : MonoBehaviour
    {
        public static Action OnClose;

        public void OnCloseButton() => OnClose?.Invoke();
    }
}
