using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private List<TutorialLayout> _layouts;
        private TutorialTitle _title;
        private GameObject _viewport;
        private GraphicRaycaster _graphicRaycaster;

        private bool _open;

        private void Awake()
        {
            _layouts = GetComponentsInChildren<TutorialLayout>(true).ToList();
            _layouts.ForEach(x => x.gameObject.SetActive(false));
            _title = GetComponentInChildren<TutorialTitle>();

            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _graphicRaycaster.enabled = false;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public bool Closed() => !_open;

        public void FillUi(TutorialPanel tutorial)
        {
            _title.SetTitle(tutorial.GetTitle());

            Sprite image = tutorial.GetImage();
            foreach (TutorialLayout layout in _layouts)
            {
                layout.SetImage(image);
                layout.SetActive(image);
                layout.SetText(tutorial.GetText());
            }
        }

        public void Open()
        {
            if (_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 1f).SetEase(Ease.InQuad);
            _graphicRaycaster.enabled = true;
            
            _gameManager.SetTutorialOpen(true);
            _gameManager.OnUIOpen();
        }

        public void Close(bool resume)
        {
            if (!_open) return;
            _graphicRaycaster.enabled = false;

            _viewport.transform.DOMoveX(Screen.width * -0.5f, 1f).SetEase(Ease.InQuad).onComplete += () =>
            {
                _viewport.SetActive(false);
                _open = false;
                
                if (resume) _gameManager.OnUIClose();
                _gameManager.SetTutorialOpen(false);
            };
        }
    }
}
