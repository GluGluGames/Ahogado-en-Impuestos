using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Museum
{
    [RequireComponent(typeof(Button))]
    public class MuseumExitButton : MonoBehaviour
    {
        public Action OnExit;

        public void OnExitButton()
        {
            OnExit?.Invoke();
        }
    }
}
