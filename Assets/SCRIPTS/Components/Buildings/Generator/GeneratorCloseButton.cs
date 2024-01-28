using System;
using UnityEngine;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorCloseButton : MonoBehaviour
    {
        public static Action OnClose;

        public void OnCloseButton() => OnClose?.Invoke();
    }
}
