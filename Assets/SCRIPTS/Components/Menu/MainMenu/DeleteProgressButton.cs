using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class DeleteProgressButton : MonoBehaviour
    {
        public Action OnDeleteProgress;

        public void OnButtonPress()
        {
            OnDeleteProgress?.Invoke();
        }

    }
}
