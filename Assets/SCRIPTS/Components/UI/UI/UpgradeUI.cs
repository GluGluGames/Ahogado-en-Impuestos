using DG.Tweening;
using GGG.Components.Buildings;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button SellButton;
        [SerializeField] private Button UpgradeButton;
        [SerializeField] private Button InteractButton;

        private GameObject _panel;
        private BuildButton[] _buttons;
        private Transform _transform;
        private HexTile _selectedTile;

        private bool _open;
        
        public Action OnMenuOpen;

        private void Start()
        {
            _panel = transform.GetChild(0).gameObject;
            _panel.SetActive(false);
            CloseButton.gameObject.SetActive(false);
            _transform = transform;
            _transform.position = new Vector3(960, -360);

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
            _transform.DOMove(new Vector3(960, 540), 0.1f).SetEase(Ease.InCubic);
            OnMenuOpen?.Invoke();
        }

        public void Close()
        {
            if(!_open) return;

            _transform.DOMove(new Vector3(960, -360), 0.1f).SetEase(Ease.InCubic).onComplete += () => 
            {
                _panel.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };
            
            _selectedTile.DeselectTile();
            _selectedTile = null;
            _open = false;
        }
    }
}
