using System;
using DG.Tweening;
using GGG.Components.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private GameObject Viewport;
        [Header("Tutorial Fields")]
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private Image TutorialImage;
        [SerializeField] private TMP_Text TutorialText;
        [SerializeField] private Button ContinueButton;
        
        private bool _open;
        public Action OnContinueButton;

        private void Start()
        {
            Viewport.transform.position = new Vector3(Screen.width * -1.3f, Screen.height * 0.5f);
        }

        public void SetTutorialFields(string title, Sprite image, string text)
        {
            TitleText.SetText(title);
            TutorialImage.sprite = image;
            TutorialText.SetText(text);
        }

        public void OnContinue() => OnContinueButton.Invoke();

        public void Open()
        {
            if (_open) return;

            _open = true;
            Viewport.SetActive(true);
            GameManager.Instance.OnUIOpen();

            Viewport.transform.DOMoveX(Screen.width * 0.5f, 1.5f).SetEase(Ease.InQuad);
        }

        public void Close()
        {
            if (!_open) return;

            Viewport.transform.DOMoveX(Screen.width * -1.3f, 1.5f).SetEase(Ease.OutQuad).onComplete += () =>
            {
                _open = false;
                Viewport.SetActive(false);
            };
            
            GameManager.Instance.OnUIClose();
        }
    }
}
