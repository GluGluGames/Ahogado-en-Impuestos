using System;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Shared;
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
            Viewport.transform.position = new Vector3(Screen.width * -0.3f, Screen.height * 0.5f);
        }

        public void SetTutorialFields(string title, Sprite image, string text)
        {
            TitleText.SetText(!string.IsNullOrEmpty(title) ? title : "");
            TutorialImage.gameObject.SetActive(image);
            if(image) TutorialImage.sprite = image;
            TutorialText.SetText(!string.IsNullOrEmpty(text) ? text : "");
        }

        public void OnContinue() => OnContinueButton.Invoke();

        public void Open()
        {
            if (_open) return;

            _open = true;
            Viewport.SetActive(true);
            GameManager.Instance.OnUIOpen();
            GameManager.Instance.SetTutorialOpen(true);
            
            Viewport.transform.DOMoveX(Screen.width * 0.5f, 1f).SetEase(Ease.InQuad);
        }

        public void Close(bool tutorialEnd, bool closeUi)
        {
            if (!_open) return;

            Viewport.transform.DOMoveX(Screen.width * -0.3f, 1f).SetEase(Ease.OutQuad).onComplete += () =>
            {
                _open = false;
                Viewport.SetActive(false);

                if (tutorialEnd) GameManager.Instance.SetCurrentTutorial(Tutorials.None);
                if (closeUi) GameManager.Instance.OnUIClose();
                GameManager.Instance.SetTutorialOpen(false);
            };
            
            
        }
    }
}
