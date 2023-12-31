using System;
using DG.Tweening;
using GGG.Classes.Tutorial;
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
        [SerializeField] private GameObject[] Layouts;
        [SerializeField] private GameObject ObjectivesPanel;
        [Header("Tutorial Fields")]
        [SerializeField] private GameObject TitleContainer;
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text[] TutorialText;
        [SerializeField] private TMP_Text[] Objectives;
        [SerializeField] private Image TutorialImage;
        
        private bool _open;
        public Action OnContinueButton;

        private void Start()
        {
            Viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            Viewport.SetActive(false);
            ObjectivesPanel.SetActive(false);
            foreach (TMP_Text objective in Objectives)
                objective.gameObject.SetActive(false);
        }

        public void SetTutorialFields(string title, Sprite image, string text)
        {
            if (!string.IsNullOrEmpty(title))
            {
                TitleContainer.gameObject.SetActive(true);
                TitleText.SetText(title);
            }
            else TitleContainer.gameObject.SetActive(false);
            
            Layouts[0].SetActive(image);
            Layouts[1].SetActive(!image);
            
            TutorialImage.gameObject.SetActive(image);
            TutorialText[image ? 0 : 1].SetText(!string.IsNullOrEmpty(text) ? text : "");
            
            if (image)
            {
                TutorialImage.sprite = image;
            }
        }

        public void SetObjectives(TutorialObjective objectives)
        {
            if (objectives == null)
            {
                ObjectivesPanel.SetActive(false);
                foreach (TMP_Text objective in Objectives)
                    objective.gameObject.SetActive(false);
                
                return;
            }
            
            ObjectivesPanel.SetActive(true);
            for (int i = 0; i < objectives.GetObjectives.Length; i++)
            {
                Objectives[i].gameObject.SetActive(true);
                Objectives[i].SetText(objectives.Objective(i).GetLocalizedString());
            }
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

            Viewport.transform.DOMoveX(Screen.width * -0.5f, 1f).SetEase(Ease.OutQuad).onComplete += () =>
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
