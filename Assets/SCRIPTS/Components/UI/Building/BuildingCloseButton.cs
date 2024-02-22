using System;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    public class BuildingCloseButton : MonoBehaviour
    {
        public Action OnCloseButton;
        
        public void OnClose()
        {
            OnCloseButton?.Invoke();
        }
    }
}
