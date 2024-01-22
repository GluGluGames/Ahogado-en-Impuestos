using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Tutorial
{
    [RequireComponent(typeof(Button))]
    public class TutorialContinueButton : MonoBehaviour
    {
        public Action OnContinue;
        
        public void OnContinueButton()
        {
            OnContinue?.Invoke();
        }
    }
}
