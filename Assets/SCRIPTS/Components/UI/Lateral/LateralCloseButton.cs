using System;
using UnityEngine;

namespace GGG.Components.UI.Lateral
{
    public class LateralCloseButton : MonoBehaviour
    {
        public Action OnLateralUI;

        public void OnPress() => OnLateralUI?.Invoke();

        public void ChangeRotation(Quaternion rotation) => transform.rotation = rotation;
        
    }
}
