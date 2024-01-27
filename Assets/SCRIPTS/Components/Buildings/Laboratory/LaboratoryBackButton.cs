using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    [RequireComponent(typeof(Button))]
    public class LaboratoryBackButton : MonoBehaviour
    {
        public Action OnBack;

        public void OnBackButton() => OnBack?.Invoke();
    }
}
