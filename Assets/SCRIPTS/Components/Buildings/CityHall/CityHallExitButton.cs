using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    [RequireComponent(typeof(Button))]
    public class CityHallExitButton : MonoBehaviour
    {
        public Action OnExit;

        public void OnExitButton() => OnExit?.Invoke();
    }
}
