using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Core;
using UnityEngine;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.Buildings
{
    public class FarmUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] Panels;
        [SerializeField] private Image SelectedImage;
        [SerializeField] private Button CloseButton;

        private GameManager _gameManager;
        
        private Dictionary<int, List<Resource>> _resources = new();
        private List<Button> _buttons = new();
        private GameObject _viewport;
        
        private int _active = 0;
        private bool _open;

        public static Action OnFarmUIOpen;
        
        private void Awake()
        {
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources").ToList());
            _resources.Add(1, Resources.LoadAll<Resource>("FishResources").ToList());
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            _gameManager = GameManager.Instance;
            
            List<Button[]> auxButtons = new ();
            foreach (GameObject panel in Panels) auxButtons.Add(panel.GetComponentsInChildren<Button>());

            foreach (Button[] buttons in auxButtons)
            {
                foreach (Button button in buttons)
                {
                    _buttons.Add(button);
                }
            }
            
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void InitializeButtons(int resource)
        {
            int a = _buttons.Count / 2 - 1;
            
            for (int i = 0; i < _buttons.Count; i++)
            {
                int idx = i % 2 == 0 ? i / 2 : i + a--;
                
                if(_resources[resource].Count <= i) _buttons[idx].transform.parent.gameObject.SetActive(false);
                else
                {
                    SpriteState aux = new()
                    {
                        selectedSprite = _resources[resource][i].GetSelectedSprite(),
                        highlightedSprite = _resources[resource][i].GetSelectedSprite(),
                        disabledSprite = _resources[resource][i].GetSprite()
                    };

                    _buttons[idx].spriteState = aux;
                    _buttons[idx].image.sprite = _buttons[idx].spriteState.disabledSprite;

                    int index = idx;
                    _buttons[idx].onClick.AddListener(() => SelectResource(index));
                }
            }
            //SelectResource(_active);
        }
        
        private void SelectResource(int index)
        {
            _buttons[index].image.sprite = _buttons[index].spriteState.disabledSprite;
            _active = index;
            _buttons[index].image.sprite = _buttons[index].spriteState.selectedSprite;

            SelectedImage.sprite = _buttons[index].spriteState.disabledSprite;
        }
        
        public void Open(FarmTypes type)
        {
            if(_open) return;

            _viewport.SetActive(true);
            
            InitializeButtons((int) type);
            OnFarmUIOpen?.Invoke();
            
            _gameManager.OnUIOpen();
            _open = true;
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f, true).SetEase(Ease.OutCubic).onComplete += () => { 
                _viewport.SetActive(false);
                _open = false;
            };
            
            _gameManager.OnUIClose();
        }
    }
}
