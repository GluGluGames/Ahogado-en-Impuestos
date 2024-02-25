using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Upgrade
{
    [RequireComponent(typeof(Button))]
    public class UpgradeCloseButton : MonoBehaviour
    {
        public static Action OnCloseButton;

        public void OnClose() => OnCloseButton?.Invoke();
    }
}
