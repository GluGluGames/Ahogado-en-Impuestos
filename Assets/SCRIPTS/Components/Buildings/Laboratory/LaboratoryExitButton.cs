using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    [RequireComponent(typeof(Button))]
    public class LaboratoryExitButton : MonoBehaviour
    {
        public Action OnExit;

        public void OnExitButton() => OnExit?.Invoke();
    }
}
