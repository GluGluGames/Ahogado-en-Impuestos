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
        
        private readonly Dictionary<int, List<Resource>> _resources = new();
        private readonly List<Button> _buttons = new();
        private GameObject _viewport;

        private Button _buttonActive;
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

        private void InitializeButtons(int resource, Resource currentResource, Farm farm)
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
                    _buttons[idx].interactable = _resources[resource][i].Unlocked();
                    _buttons[idx].image.color = _resources[resource][i].Unlocked() ? Color.white : Color.black;

                    int button = idx;
                    int res = i;
                    _buttons[idx].onClick.AddListener(() => SelectResource(button, _resources[resource][res], farm));

                    if (!currentResource)
                    {
                        SelectedImage.color = new Color(1f, 1f, 1f, 0f);
                        continue;
                    }

                    if (currentResource != _resources[resource][i]) continue;
                    
                    SelectResource(idx, _resources[resource][i], farm);
                    SelectedImage.sprite = currentResource.GetSprite();
                }
            }
        }
        
        private void SelectResource(int index, Resource resource, Farm farm)
        {
            if (_buttons[index] == _buttonActive) return;
            
            if (_buttonActive)
                _buttonActive.image.sprite = _buttonActive.spriteState.disabledSprite;
            
            _buttons[index].image.sprite = _buttons[index].spriteState.selectedSprite;
            _buttonActive = _buttons[index];

            SelectedImage.sprite = _buttons[index].spriteState.disabledSprite;
            SelectedImage.color = Color.white;
            
            if (farm.GetResource() == resource) return;
            farm.Resource(resource);
        }
        
        public void Open(FarmTypes type, Resource currentResource, Farm farm)
        {
            if(_open) return;

            _viewport.SetActive(true);
            
            InitializeButtons((int) type, currentResource, farm);
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
