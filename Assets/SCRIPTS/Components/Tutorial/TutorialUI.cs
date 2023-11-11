using DG.Tweening;
using GGG.Components.Core;
using UnityEngine;

namespace GGG.Components.Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private GameObject Viewport;
        
        private bool _open;

        private void Start()
        {
            Viewport.transform.position = new Vector3(Screen.width * -1.3f, Screen.height * 0.5f);
        }

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
