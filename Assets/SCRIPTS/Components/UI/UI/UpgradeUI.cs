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
        private BuildingComponent _auxBuilding;
        private Transform _transform;

        public Action OnMenuClose;

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
                tile.OnHexSelect += () => {
                    if (tile.GetCurrentBuilding() != _auxBuilding) {
                        Close();
                    }
                };
                OnMenuClose += tile.DeselectTile;
            }

            SellButton.onClick.AddListener(OnSellButton);
            UpgradeButton.onClick.AddListener(OnUpgradeButton);
            CloseButton.onClick.AddListener(Close);
        }

        private void UpdateBuildings(BuildingComponent building)
        {
            building.OnBuildInteract += OnBuildInteract;
        }

        private void OnBuildInteract(Action action, BuildingComponent build) 
        {
            _transform.DOMove(new Vector3(960, 540), 0.1f).SetEase(Ease.InCubic);
            _auxBuilding = build;
            _panel.SetActive(true);
            CloseButton.gameObject.SetActive(true);
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

        private void Close()
        {
            _transform.DOMove(new Vector3(960, -360), 0.1f).SetEase(Ease.InCubic).onComplete += () => 
            {
                _panel.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };
            
            _auxBuilding = null;
            OnMenuClose?.Invoke();
        }
    }
}
