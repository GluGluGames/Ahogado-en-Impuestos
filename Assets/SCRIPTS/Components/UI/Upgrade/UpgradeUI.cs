using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using GGG.Components.Core;
using GGG.Components.UI;
using GGG.Shared;
using GGG.Input;

using System;
using System.Linq;
using TMPro;
using DG.Tweening;
using GGG.Components.Serialization;
using GGG.Components.UI.Buildings;
using GGG.Components.UI.Upgrade;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        private UpgradeInteractButton _interactButton;
        private UpgradeSellButton _sellButton;
        private UpgradeButton _upgradeButton;
        
        private InputManager _input;
        private GameManager _gameManager;
        private GameObject _viewport;

        private bool _open;

        public Action<HexTile> OnUiOpen;
        public Action OnUIClose;

        private void Awake()
        {
            UpgradeCloseButton.OnCloseButton += OnCloseButton;
            
            _interactButton = GetComponentInChildren<UpgradeInteractButton>();
            _interactButton.OnBuildInteract += Close;

            _sellButton = GetComponentInChildren<UpgradeSellButton>();
            _sellButton.OnBuildSell += Close;

            _upgradeButton = GetComponentInChildren<UpgradeButton>();
            _upgradeButton.OnBuildUpgrade += Close;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);

            TilesSerialization.OnBuildingTileLoaded += UpdateBuildings;
        }

        private void Start()
        {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            FindObjectsOfType<BuildButton>().ToList().ForEach(x =>
            {
                x.OnStructureBuild += UpdateBuildings;
            });
        }

        private void Update()
        {
            if (_open && _input.Escape() && !_gameManager.OnTutorial()) Close();
        }

        private void OnDisable()
        {
            UpgradeCloseButton.OnCloseButton -= OnCloseButton;
            _interactButton.OnBuildInteract -= Close;
            _sellButton.OnBuildSell -= Close;
            _upgradeButton.OnBuildUpgrade -= Close;
            
            FindObjectsOfType<BuildButton>().ToList().ForEach(x =>
            {
                x.OnStructureBuild -= UpdateBuildings;
            });
        }
        
        private void UpdateBuildings(BuildingComponent build, HexTile tile)
        {
            build.OnBuildSelect += (x) =>
            {
                Open(tile);
            };
        }

        private void Open(HexTile tile) {
            if(_open) return;

            _open = true;
            
            OnUiOpen?.Invoke(tile);
            _gameManager.OnUIOpen();
            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if(!_open || _gameManager.TutorialOpen()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => 
            {
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                _open = false;
            };
            
            OnUIClose?.Invoke();
        }
    }
}
