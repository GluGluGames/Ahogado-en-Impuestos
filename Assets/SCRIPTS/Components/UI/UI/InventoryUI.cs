using System;
using System.Collections;
using GGG.Shared;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Components.UI.Buttons;
using GGG.Input;

namespace GGG.Components.UI
{
    public class InventoryUI : MonoBehaviour
    {
        #region Public variables

        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject[] ResourceContainers;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button CloseButton;

        #endregion

        #region Private variables

        private readonly Dictionary<int, Resource[]> _resources = new();
        private readonly Dictionary<int, Button[]> _buttons = new();
        private List<ContainerButton> _containerButtons;

        private Dictionary<string, TextMeshProUGUI> _resourcesCountText;

        private PlayerManager _player;
        private InputManager _input;
        private HUDManager _hudManager;
        private GameManager _gameManager; 
            
        private GameObject _viewport;
        private bool _open;

        #endregion

        #region Unity functions

        private void Awake()
        {
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _hudManager = HUDManager.Instance;
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);

            StartCoroutine(Initialize());
        }

        private void Update()
        {
            if (!_open) return;

            UpdateResourcesAmount();
            
            if (!_input.Escape() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _containerButtons.Count; i++)
            {
                int idx = i;
                for (int j = 0; j < _containerButtons.Count - 1; j++)
                {
                    _containerButtons[i].OnButtonClick -=
                        _containerButtons[(idx + 1) % _containerButtons.Count].DeselectButton;
                    idx++;
                }
            }
        }

        #endregion

        #region Methods

        private IEnumerator Initialize()
        {
            _resourcesCountText = new Dictionary<string, TextMeshProUGUI>(_player.GetResourceNumber());
            
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources"));
            _resources.Add(1, Resources.LoadAll<Resource>("ExpeditionResources"));
            _resources.Add(2, Resources.LoadAll<Resource>("FishResources"));

            while (_resources[0].Length <= 0 || _resources[1].Length <= 0 || _resources[2].Length <= 0)
                yield return null;

            _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();
            for (int i = 0; i < _containerButtons.Count; i++)
            {
                int idx = i;
                for (int j = 0; j < _containerButtons.Count - 1; j++)
                {
                    _containerButtons[i].OnButtonClick +=
                        _containerButtons[(idx + 1) % _containerButtons.Count].DeselectButton;
                    idx++;
                }
            }

            for (int i = 0; i < _resources.Count; i++)
            {
                _buttons.Add(i, ResourceContainers[i].GetComponentsInChildren<Button>());
                FillResources(_buttons[i], _resources[i]);
            }
            
            ResetContainers();
        }

        private void ResetContainers()
        {
            for (int i = 0; i < _containerButtons.Count; i++)
            {
                _containerButtons[i].Initialize();
                ResourceContainers[i].SetActive(i == 0);
            }
        }

        private void UpdateResourcesAmount()
        {
            foreach (string key in _resourcesCountText.Keys)
            {
                if (_player.GetResourceCount(key) == 0) continue;
                
                _resourcesCountText[key].SetText(_player.GetResourceCount(key).ToString());
            }
        }
        
        private void FillResources(Button[] buttons, Resource[] resources)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (resources.Length <= i) buttons[i].transform.parent.gameObject.SetActive(false);
                else
                {
                    buttons[i].image.sprite = resources[i].GetSprite();
                    SpriteState aux = new()
                    {
                        highlightedSprite = resources[i].GetSelectedSprite()
                    };
                    
                    buttons[i].spriteState = aux;
                    int index = i;
                    buttons[i].onClick.AddListener(() => AddListener(resources[index], buttons[index]));
                    _resourcesCountText[resources[i].GetKey()] =
                        buttons[i].transform.GetComponentInChildren<TextMeshProUGUI>(true);
                }
            }
        }

        private void AddListener(Resource resource, Button button)
        {
            if (resource.GetKey().Equals("Seaweed")) return;
            
            if (_hudManager.ResourceBeingShown(resource))
            {
                if(_hudManager.HideResource(resource))
                    button.image.sprite = resource.GetSprite();
                return;
            }
            
            if(_hudManager.ShowResource(resource))
                button.image.sprite = resource.GetSelectedSprite();
        }

        private void HandleSelectedResources()
        {
            for (int i = 0; i < _resources.Count; i++)
            {
                for (int j = 0; j < _resources[i].Length; j++)
                {
                    _buttons[i][j].image.sprite = _hudManager.ResourceBeingShown(_resources[i][j])
                        ? _resources[i][j].GetSelectedSprite() : _resources[i][j].GetSprite();
                }
            }
        }

        public void OpenInventory()
        {
            if (_open) return;

            _viewport.SetActive(true);
            _open = true;
            _gameManager.OnUIOpen();
            
            HandleSelectedResources();
            ResetContainers();
            
            foreach (string key in _resourcesCountText.Keys)
                _resourcesCountText[key].SetText(_player.GetResourceCount(key).ToString());
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.5f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.5f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                _open = false;
            };
        }

        #endregion
    }
}
