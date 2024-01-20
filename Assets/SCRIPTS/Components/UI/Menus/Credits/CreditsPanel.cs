using System;
using DG.Tweening;
using UnityEngine;

namespace GGG.Components.Menus
{
    public class CreditsPanel : MonoBehaviour
    {
        [SerializeField] private int Duration;

        private Credits _credits;
        private CreditsCloseButton _closeButton;
        private CreditsButton _creditsButton;
        private Transform _transform;
        
        private void OnEnable()
        {
            _transform = transform;
            _transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.6f);
            
            _credits = FindObjectOfType<Credits>();
            
            _creditsButton = FindObjectOfType<CreditsButton>(true);
            _creditsButton.OnCredits += StartAnimation;
            
            _closeButton = FindObjectOfType<CreditsCloseButton>();
            _closeButton.OnClose += StopAnimation;
        }

        private void OnDisable()
        {
            _closeButton.OnClose -= StopAnimation;
        }

        private void StartAnimation()
        {
            transform.DOMoveY(Screen.height * 4.8f, Duration).SetEase(Ease.Linear).onComplete += 
                () => StartCoroutine(_credits.CloseDelay(2));
        }

        private void StopAnimation()
        {
            _transform.DOKill();
            _transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.6f);
        }
    }
}
