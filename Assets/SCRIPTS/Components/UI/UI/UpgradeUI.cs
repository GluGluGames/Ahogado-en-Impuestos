using GGG.Components.Buildings;
using GGG.Components.Player;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using GGG.Components.Core;
using GGG.Shared;
using System.Collections.Generic;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button SellButton;
        [SerializeField] private Button UpgradeButton;
        [SerializeField] private Button InteractButton;

        [SerializeField] private GameObject InteractParent;

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

            TileManager.OnTilesStateLoaded += UpdateBuildings;

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                // tile.OnHexDeselect += Close;
            }

            SellButton.onClick.AddListener(OnSellButton);
            UpgradeButton.onClick.AddListener(OnUpgradeButton);
            CloseButton.onClick.AddListener(() => Close(true));
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

        private void UpdateBuildings(BuildingComponent[] buildings)
        {
            foreach (BuildingComponent building in buildings)
            {
                building.OnBuildInteract += (x, y) =>
                {
                    OnBuildInteract(x, y);
                    _selectedBuilding = building;
                    Open(building.GetCurrentTile());
                };
            }
        }

        private void OnBuildInteract(Action action, BuildingComponent build) 
        {
            InteractButton.onClick.AddListener(() => {
                action.Invoke();
                Close(false);
            });

            if (!build.NeedInteraction()) InteractParent.SetActive(false); //InteractButton.gameObject.SetActive(false);
            else InteractParent.SetActive(true); //InteractButton.gameObject.SetActive(true); 
        }

        private void OnSellButton()
        {
            // TODO - Implement sell button
            _player.AddResource(_sellResource.GetKey(), Mathf.RoundToInt(_selectedBuilding.GetBuildCost() * 0.5f));
            _selectedTile.DestroyBuilding();
            
            _selectedBuilding = null;
            Close(true);
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

        public void Close(bool resumeGame)
        {
            if(!_open) return;

            _transform.DOMove(new Vector3(Screen.width * 0.5f, -360), 0.1f).SetEase(Ease.InCubic).onComplete += () => 
            {
                _panel.SetActive(false);
                CloseButton.gameObject.SetActive(false);
                InteractButton.onClick.RemoveAllListeners();
            };
            
            _selectedTile.DeselectTile();
            _selectedTile = null;
            if(resumeGame) GameManager.Instance.OnUIClose();
            _open = false;
        }
    }
}
