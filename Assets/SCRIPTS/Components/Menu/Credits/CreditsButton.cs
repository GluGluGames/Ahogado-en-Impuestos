using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class CreditsButton : MonoBehaviour
    {
        public Action OnCredits;

        public void OnCreditsButton()
        {
            OnCredits?.Invoke();
        }
    }
}
