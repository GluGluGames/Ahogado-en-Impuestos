using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Shop
{
    [RequireComponent(typeof(Button))]
    public class ShopExitButton : MonoBehaviour
    {
        public Action OnExit;

        public void OnExitButton()
        {
            OnExit?.Invoke();
        }
    }
}
