using System;
using UnityEngine;
using DG.Tweening;
using GGG.Components.Core;
using UnityEngine.UI;

namespace GGG.Components.Dialogue {
    public class DialogueAnimation : MonoBehaviour {
        [SerializeField] private GameObject Viewport;
        [SerializeField] private Image Avatar;
        private DialogueBox _dialogue;

        private void Start() {
            _dialogue = GetComponentInChildren<DialogueBox>(true);
            Viewport.SetActive(false);

            _dialogue.DialogueStart += DialogueOpen;
            _dialogue.DialogueEnd += DialogueClose;
            
            Viewport.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.7f);
            Avatar.color = new Color(1f, 1f, 1f, 0f);
        }

        private void OnDisable() {
            _dialogue.DialogueStart -= DialogueOpen;
            _dialogue.DialogueEnd -= DialogueClose;
        }

        private void DialogueOpen() {
            Viewport.SetActive(true);
            Viewport.transform.DOMoveY(0, 1f).SetEase(Ease.InSine);
            Avatar.DOFade(1f, 2f);
            GameManager.Instance.OnUIOpen();
        }

        private void DialogueClose() {

            Avatar.DOFade(0f, 0.5f);
            Viewport.transform.DOMoveY(Screen.height * -0.7f, 1f).SetEase(Ease.OutSine).onComplete += () => { 
                Viewport.SetActive(false);
            };

            GameManager.Instance.OnUIClose();
        }
    }
}
