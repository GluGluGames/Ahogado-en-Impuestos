using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace GGG.Components.Museum
{
    public class MuseumUI : MonoBehaviour
    {
        #region Public variables

        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject[] ResourcesContainers;

        [Space(5), Header("Information")] 
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI Description;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button[] ContainerButtons;
        [SerializeField] private Button CloseButton;
        
        [SerializeField] private LocalizedString LockedString;

        public static Action OnMuseumOpen;

        #endregion

        #region Private variables

        private GameManager _gameManager;

        private readonly Dictionary<int, Resource[]> _resources = new();
        private readonly Dictionary<int, Button[]> _buttons = new();
        private readonly Dictionary<int, bool[]> _buttonActive = new();

        private GameObject _viewport;
        
        private int _active;
        private readonly int[] _activeResource = { 0, 0, 0 };
        private bool _open;
        
        #endregion

        #region Unity functions

        private void Awake()
        {
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources"));
            _resources.Add(1, Resources.LoadAll<Resource>("ExpeditionResources"));
            _resources.Add(2, Resources.LoadAll<Resource>("FishResources"));

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;

            Initialize();

            CheckResources();

            CloseButton.onClick.AddListener(OnCloseButton);
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            for (int i = 0; i < ResourcesContainers.Length; i++)
            {
                _buttons.Add(i, ResourcesContainers[i].GetComponentsInChildren<Button>());
                _buttonActive.Add(i, new bool[_resources[i].Length]);
                InitArray(_buttonActive[i], _buttons[i]);
                
                int index = i;
                ContainerButtons[i].onClick.AddListener(() => 
                    HandleToggle(_resources[index], _buttons[index], _buttonActive[index], index));
            }

            for (int i = 0; i < _resources.Count; i++)
                if(_resources[i].Length > 0)
                    FillResources(_buttons[i], _resources[i], _buttonActive[i], i);
        }

        private void FillResources(Button[] buttons, Resource[] resources, bool[] active, int type)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (resources.Length <= i) buttons[i].transform.parent.gameObject.SetActive(false);
                else
                {
                    buttons[i].image.sprite = resources[i].GetSprite();
                    SpriteState aux = new()
                    {
                        selectedSprite = resources[i].GetSelectedSprite(),
                        highlightedSprite = resources[i].GetSelectedSprite(),
                        disabledSprite = resources[i].GetSprite()
                    };
                    
                    buttons[i].spriteState = aux;
                    int index = i;
                    buttons[i].onClick.AddListener(() => SelectResource(resources, buttons, active, type, index));
                }
            }
        }

        private void HandleToggle(Resource[] resources, Button[] buttons, bool[] active, int index)
        {
            if (_active == index) return;

            for (int i = 0; i < ContainerButtons.Length; i++)
            {
                ContainerButtons[i].image.sprite = i == index
                    ? ContainerButtons[i].spriteState.selectedSprite
                    : ContainerButtons[i].spriteState.disabledSprite;

                ResourcesContainers[i].SetActive(i == index);
            }

            SelectResource(resources, buttons, active, index, 0);

            _active = index;
        }

        private void InitArray(bool[] array, Button[] buttons)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = false;
                buttons[i].image.color = Color.black;
            }
        }

        private void UnlockResource(bool[] array, Button[] buttons, int i)
        {
            array[i] = true;
            buttons[i].image.color = Color.white;
        }

        private void SelectResource(Resource[] resources, Button[] buttons, bool[] isActive, int type, int i)
        {
            int activeRes = _activeResource[type];
            if (activeRes == i && activeRes != 0) return;
            
            buttons[activeRes].image.sprite = buttons[activeRes].spriteState.disabledSprite;
            
            Name.text = isActive[i] ? resources[i].GetName() : LockedString.GetLocalizedString();
            Description.text = isActive[i] ? resources[i].GetDescription() : LockedString.GetLocalizedString();
            
            buttons[i].image.sprite = buttons[i].spriteState.selectedSprite;
            _activeResource[type] = i;
        }

        private void CheckResources()
        {
            for (int i = 0; i < _resources.Count; i++)
            {
                for (int j = 0; j < _resources[i].Length; j++)
                    if (_resources[i][j].Unlocked())
                        UnlockResource(_buttonActive[i], _buttons[i], j);
            }
        }

        public void Open()
        {
            if (_open) return;
            
            _open = true;
            _viewport.SetActive(true);
            
            CheckResources();
            SelectResource(_resources[0], _buttons[0], _buttonActive[0], 0, 0);
            ContainerButtons[0].image.sprite = ContainerButtons[0].spriteState.selectedSprite;
            
            OnMuseumOpen?.Invoke();
            _gameManager.OnUIOpen();
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
            };
        }

        #endregion
    }
}
