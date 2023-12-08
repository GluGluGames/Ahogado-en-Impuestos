using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GGG.Classes.Buildings;
using GGG.Components.UI.Buttons;
using Project.Component.UI.Containers;

namespace GGG.Components.Buildings.Museum
{
    public class MuseumUI : MonoBehaviour
    {
        #region Public variables

        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject[] ResourcesContainers;
        [SerializeField] private GameObject BuildingsContainer;

        [Space(5), Header("Information")] 
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI Description;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button CloseButton;
        
        [SerializeField] private LocalizedString LockedString;

        public static Action OnMuseumOpen;

        #endregion

        #region Private variables

        private GameManager _gameManager;

        private readonly Dictionary<int, Resource[]> _resources = new();
        private readonly Dictionary<int, Button[]> _buttons = new();
        private readonly Dictionary<int, bool[]> _buttonActive = new();
        private Building[] _buildings;
        private List<ContainerButton> _containerButtons;

        private GameObject _viewport;
        
        private int _active;
        private readonly int[] _activeResource = { 0, 0, 0 };
        private int _activeBuilding;
        private bool _open;
        
        #endregion

        #region Unity functions

        private void Awake()
        {
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources"));
            _resources.Add(1, Resources.LoadAll<Resource>("ExpeditionResources"));
            _resources.Add(2, Resources.LoadAll<Resource>("FishResources"));
            _buildings = Resources.LoadAll<Building>("Buildings");

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

        private void OnDisable()
        {
            _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();
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

        private void Initialize()
        {
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

            int lastIdx = 0;
            
            for (int i = 0; i < ResourcesContainers.Length; i++)
            {
                _buttons.Add(i, ResourcesContainers[i].GetComponentsInChildren<Button>());
                _buttonActive.Add(i, new bool[_resources[i].Length]);
                InitArray(_buttonActive[i], _buttons[i]);
                int index = i;
                _containerButtons[i].OnButtonClick += () =>
                    SelectResource(_resources[index], _buttons[index], _buttonActive[index], index, 0);
                lastIdx++;
            }

            _buttons.Add(lastIdx, BuildingsContainer.GetComponentsInChildren<Button>());
            _buttonActive.Add(lastIdx, new bool[_buildings.Length]);
            InitArray(_buttonActive[lastIdx], _buttons[lastIdx]);
            _containerButtons[lastIdx].OnButtonClick += () =>
                SelectBuilding(_buildings, _buttons[lastIdx], _buttonActive[lastIdx], lastIdx);

            for (int i = 0; i < _resources.Count; i++)
                if(_resources[i].Length > 0)
                    FillResources(_buttons[i], _resources[i], _buttonActive[i], i);
            
            FillBuildings(_buttons[lastIdx], _buildings, _buttonActive[lastIdx]);
        }
        
        private void ResetContainers()
        {
            for (int i = 0; i < _containerButtons.Count - 1; i++)
            {
                _containerButtons[i].Initialize();
                ResourcesContainers[i].SetActive(i == 0);
            }
            
            _containerButtons[^1].Initialize();
            BuildingsContainer.SetActive(false);
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
                    buttons[i].transform.parent.GetComponent<Tooltip>().SetResourceName(resources[i].GetName());
                    int index = i;
                    buttons[i].onClick.AddListener(() => SelectResource(resources, buttons, active, type, index));
                }
            }
        }

        private void FillBuildings(Button[] buttons, Building[] buildings, bool[] active)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if(buildings.Length <= i) buttons[i].transform.parent.gameObject.SetActive(false);
                else
                {
                    buttons[i].image.sprite = buildings[i].GetResearchIcon();
                    SpriteState aux = new()
                    {
                        selectedSprite = buildings[i].GetSelectedIcon(),
                        highlightedSprite = buildings[i].GetSelectedIcon(),
                        disabledSprite = buildings[i].GetResearchIcon()
                    };

                    buttons[i].spriteState = aux;
                    int idx = i;
                    buttons[i].onClick.AddListener(() => SelectBuilding(buildings, buttons, active, idx));
                }
            }
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

        private void UnlockBuilding(bool[] array, Button[] buttons, int i)
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

        private void SelectBuilding(Building[] buildings, Button[] buttons, bool[] isActive, int i)
        {
            if (_activeBuilding == i && _activeBuilding != 0) return;

            buttons[_activeBuilding].image.sprite = buttons[_activeBuilding].spriteState.disabledSprite;

            Name.text = isActive[i] ? buildings[i].GetName() : LockedString.GetLocalizedString();
            Description.text = isActive[i] ? buildings[i].GetDescription() : LockedString.GetLocalizedString();

            buttons[i].image.sprite = buttons[i].spriteState.selectedSprite;
            _activeBuilding = i;
        }

        private void CheckResources()
        {
            for (int i = 0; i < _resources.Count; i++)
            {
                for (int j = 0; j < _resources[i].Length; j++)
                    if (_resources[i][j].Unlocked())
                        UnlockResource(_buttonActive[i], _buttons[i], j);
            }
            
            for (int i = 0; i < _buildings.Length; i++)
                if(_buildings[i].IsUnlocked())
                    UnlockBuilding(_buttonActive[3], _buttons[3], i);
        }

        public void Open()
        {
            if (_open) return;
            
            _open = true;
            _viewport.SetActive(true);
            
            ResetContainers();
            CheckResources();
            SelectResource(_resources[0], _buttons[0], _buttonActive[0], 0, 0);
            
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
