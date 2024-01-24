using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Menus
{
    public class Credits : MonoBehaviour
    {
        private GameObject _viewport;
        private CreditsCloseButton _closeButton;
        private CreditsButton _creditsButton;
        
        private void Awake()
        {
            _closeButton = FindObjectOfType<CreditsCloseButton>();
            _creditsButton = FindObjectOfType<CreditsButton>(true);
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _closeButton.OnClose += Close;
            _creditsButton.OnCredits += Open;
        }

        public IEnumerator CloseDelay(int time)
        {
            yield return new WaitForSeconds(time);
            
            Close();
        }

        private void Open()
        {
            _viewport.SetActive(true);
        }

        private void Close()
        {
            _viewport.SetActive(false);
        }
    }
}
