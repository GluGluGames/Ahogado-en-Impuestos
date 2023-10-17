using UnityEngine;
using DG.Tweening;
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

            transform.position = new Vector3(0f, -400f, 0f);
            Avatar.color = new Color(1f, 1f, 1f, 0f);
        }

        private void DialogueOpen() {
            Viewport.SetActive(true);
            transform.DOMove(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.OutBounce);
            Avatar.DOFade(1f, 2f);
        }

        private void DialogueClose() {

            Avatar.DOFade(0f, 0.5f);
            transform.DOMove(new Vector3(0f, -400f, 0f), 0.5f).SetEase(Ease.OutFlash).onComplete += () => { 
                Viewport.SetActive(false); 
            };

        }
    }
}
