using GGG.Components.Buildings;
using GGG.Components.Player;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using GGG.Components.Core;
using GGG.Shared;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button SellButton;
        [SerializeField] private Button UpgradeButton;
        [SerializeField] private Button InteractButton;

        private PlayerManager _player;
        private Resource _sellResource;
        private GameObject _panel;
        private BuildButton[] _buttons;
        private Transform _transform;
        private HexTile _selectedTile;
        private BuildingComponent _selectedBuilding;

        private bool _open;
        
        public Action OnMenuOpen;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _sellResource = _player.GetMainResource();

            _panel = transform.GetChild(0).gameObject;
            _panel.SetActive(false);
            CloseButton.gameObject.SetActive(false);
            _transform = transform;
            _transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 360);

            _buttons = FindObjectsOfType<BuildButton>(true);

            foreach (BuildButton button in _buttons)
                button.OnStructureBuild += UpdateBuildings;

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                // tile.OnHexDeselect += Close;
            }

            SellButton.onClick.AddListener(OnSellButton);
            UpgradeButton.onClick.AddListener(OnUpgradeButton);
            CloseButton.onClick.AddListener(Close);
        }
        
        public bool IsOpen() { return _open; }

        private void UpdateBuildings(BuildingComponent building, HexTile buildingTile)
        {
            building.OnBuildInteract += (x, y) => {
                OnBuildInteract(x, y);
                _selectedBuilding = building;
                Open(buildingTile);
            };
        }

        private void OnBuildInteract(Action action, BuildingComponent build) 
        {
            InteractButton.onClick.AddListener(() => {
                action.Invoke();
                Close();
            });

            if(!build.NeedInteraction()) InteractButton.gameObject.SetActive(false);
            else InteractButton.gameObject.SetActive(true);
        }

        private void OnSellButton()
        {
            // TODO - Implement sell button
            _player.AddResource(_sellResource.GetName(), Mathf.RoundToInt(_selectedBuilding.GetBuildCost() * 0.5f));
            _selectedTile.DestroyBuilding();
            
            _selectedBuilding = null;
            Close();
        }

        private void OnUpgradeButton()
        {
            // TODO - Implement upgrade button
        }

        private void Open(HexTile tile) {
            if(_open) return;

            _open = true;
            _selectedTile = tile;
            _panel.SetActive(true);
            CloseButton.gameObject.SetActive(true);
            _transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f), 0.1f).SetEase(Ease.InCubic);
            OnMenuOpen?.Invoke();
        }

        public void Close()
        {
            if(!_open) return;

            _transform.DOMove(new Vector3(Screen.width * 0.5f, -360), 0.1f).SetEase(Ease.InCubic).onComplete += () => 
            {
                _panel.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };
            
            _selectedTile.DeselectTile();
            _selectedTile = null;
            GameManager.Instance.OnUIClose();
            _open = false;
        }
    }
}
