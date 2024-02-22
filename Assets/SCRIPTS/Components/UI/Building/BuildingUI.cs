using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Core;
using GGG.Components.UI.Buttons;
using GGG.Input;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;

namespace GGG.Components.UI 
{
    public class BuildingUI : MonoBehaviour
    {
        [Header("GameObjects")] 
        [SerializeField] private List<GameObject> Panels;
        [SerializeField] private TMP_Text PanelsText;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button RightArrow;
        [SerializeField] private Button LeftArrow;
        [SerializeField] private Button CloseButton;

        // MANAGERS
        private InputManager _input;
        private BuildingManager _buildingManager;
        private GameManager _gameManager;
        
        // OBJECTS
        private GameObject _viewport;
        private BuildButton[] _buttons;
        private HexTile _selectedTile;
        
        // VARIABLES
        private bool _open;
        private int _currentPanel;

        //EVENTS
        public static Action OnUiOpen;

        private void Start() {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            _buildingManager = BuildingManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * 0.5f, Screen.width * -0.5f, 0);
            
            Initialize();
            
            LeftArrow.onClick.AddListener(() => OnArrow(0));
            RightArrow.onClick.AddListener(() => OnArrow(1));
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Update() {
            if (!_open ) return;

            if (!_viewport.activeInHierarchy)
            {
                _viewport.SetActive(true);
            }
            
            if (!_input.Escape() || _gameManager.OnTutorial()) return;
            Close();
        }

        private void OnDisable()
        {
            LeftArrow.onClick.RemoveAllListeners();
            RightArrow.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
            
            HexTile[] tiles = FindObjectsOfType<HexTile>();
            foreach (HexTile tile in tiles) tile.OnHexSelect -= Open;
            
            _buttons = GetComponentsInChildren<BuildButton>(true);
            foreach (BuildButton button in _buttons) button.OnStructureBuild -= AuxClose;
        }

        private void Initialize()
        {
            HexTile[] tiles = FindObjectsOfType<HexTile>();
            foreach (HexTile tile in tiles) tile.OnHexSelect += Open;
            
            _buttons = GetComponentsInChildren<BuildButton>(true);
            foreach (BuildButton button in _buttons) {
                button.Initialize(_buildingManager);
                button.OnStructureBuild += AuxClose;
            }
            
            PanelsText.SetText($"{_currentPanel + 1}/{Panels.Count}");
        }
        
        private void OnArrow(int arrow)
        {
            int idx = _currentPanel + (arrow == 0 ? -1 : 1);
            
            if (idx < 0 || idx >= Panels.Count) return;
            
            Panels[_currentPanel].SetActive(false);
            Panels[idx].SetActive(true);
            _currentPanel = idx;
            PanelsText.SetText($"{_currentPanel + 1}/{Panels.Count}");
        }

        private void CheckBuildings()
        {
            foreach(BuildButton button in _buttons)
                button.CheckUnlockState();
        }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard || !tile.TileEmpty()) {
                return; 
            }
            
            _viewport.transform.DOMoveY(0, 0.75f).SetEase(Ease.InCubic).onComplete += () =>
            {
                _open = true;
            };
            
            for(int i = 0; i < Panels.Count; i++)
                Panels[i].SetActive(i == 0);
            
            _selectedTile = tile;
            _viewport.SetActive(true);
            CheckBuildings();
            OnUiOpen?.Invoke();
            _gameManager.OnUIOpen();
        }

        private void AuxClose(BuildingComponent x = null, HexTile y = null) => Close();
        
        private void OnCloseButton() {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }
        
        public void Close()
        {
            _viewport.transform.DOMoveY(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => {
                _viewport.SetActive(false);
                _open = false;
                
                _currentPanel = 0;
                for(int i = 0; i < Panels.Count; i++) Panels[i].SetActive(i == 0);
                PanelsText.SetText($"{_currentPanel + 1}/{Panels.Count}");
                
                _gameManager.OnUIClose();
            };
            
            _selectedTile.DeselectTile();
            _selectedTile = null;
        }
    }
}
