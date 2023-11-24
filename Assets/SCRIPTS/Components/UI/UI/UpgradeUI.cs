using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using GGG.Components.Core;
using GGG.Shared;
using GGG.Input;

using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [Header("Sell Fields")]
        [SerializeField] private Button SellButton;
        [SerializeField] private Image SellResource;
        [SerializeField] private TMP_Text SellCost;
        [Space(5),Header("Upgrade Fields")]
        [SerializeField] private GameObject UpgradePanel;
        [SerializeField] private Button UpgradeButton;
        [SerializeField] private Image[] UpgradeResources;
        [SerializeField] private TMP_Text[] UpgradeCost;
        [Space(5), Header("Other fields")]
        [SerializeField] private Button InteractButton;
        [SerializeField] private Button CloseButton;
        [SerializeField] private GameObject InteractParent;

        private PlayerManager _player;
        private InputManager _input;
        private GameManager _gameManager;
        private Resource _sellResource;
        private GameObject _viewport;
        private BuildButton[] _buttons;
        private HexTile _selectedTile;
        private BuildingComponent _selectedBuilding;

        private bool _open;

        public Action OnUiOpen;
        public Action OnSellButtonPress;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            _sellResource = _player.GetResource("Seaweed");

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);

            _buttons = FindObjectsOfType<BuildButton>(true);
            foreach (BuildButton button in _buttons) button.OnStructureBuild += UpdateBuildings;
            TileManager.OnBuildingTileLoaded += UpdateBuildings;

            SellButton.onClick.AddListener(OnSellButton);
            UpgradeButton.onClick.AddListener(OnUpgradeButton);
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Update()
        {
            if (!_open || !_input.Escape()) return;
            
            Close(true); 
        }

        private void OpenCheck()
        {
            int currentLevel = _selectedBuilding.CurrentLevel();
            ResourceCost[] cost = _selectedBuilding.BuildData().GetUpgradeCost();
            bool price = true;
            
            SellResource.sprite = _selectedBuilding.BuildData().GetBuildResource(0).GetSprite();
            SellCost.SetText(Mathf.RoundToInt(_selectedBuilding.CurrentCost().GetCost(0) * 0.5f).ToString());
            
            bool activeCondition = _selectedBuilding.BuildData().CanUpgraded() &&
                             currentLevel < _selectedBuilding.BuildData().GetMaxLevel();
            
            UpgradePanel.SetActive(activeCondition);

            if (!activeCondition) return;
            
            for (int i = 0; i < cost[currentLevel - 1].GetCostsAmount(); i++)
            {
                if (!_selectedBuilding.BuildData().GetUpgradeResource(currentLevel, i)) break;
                
                UpgradeCost[i].transform.parent.gameObject.SetActive(true);
                UpgradeCost[i].text = _selectedBuilding.BuildData().GetUpgradeCost(currentLevel, i).ToString();
                UpgradeResources[i].sprite = _selectedBuilding.BuildData().GetUpgradeResource(currentLevel, i).GetSprite();
                
                if (_player.GetResourceCount(cost[currentLevel - 1].GetResource(i).GetKey()) >= cost[currentLevel - 1].GetCost(i)) continue;
                
                price = false;
            }

            UpgradeButton.interactable = price;
            UpgradeButton.image.color = price ? Color.white : new Color(0.81f, 0.84f, 0.81f, 0.9f);
        }
        
        private void UpdateBuildings(BuildingComponent building, HexTile buildingTile)
        {
            building.OnBuildSelect += (x) => {
                OnBuildInteract(x);
                _selectedBuilding = building;
                Open(buildingTile);
            };
        }

        private void OnBuildInteract(BuildingComponent build)
        {
            InteractButton.onClick.AddListener(() =>
            {
                if (!_open || _gameManager.OnTutorial() || _gameManager.TutorialOpen()) return;
                
                build.Interact();
                Close(false);
            });

            InteractParent.SetActive(true);
        }

        private void OnSellButton()
        {
            if(!_open || _gameManager.TutorialOpen()) return;
            
            _player.AddResource(_sellResource.GetKey(), Mathf.RoundToInt(_selectedBuilding.CurrentCost().GetCost(0) * 0.5f));
            BuildingManager.Instance.RemoveBuilding(_selectedBuilding);
            _selectedTile.DestroyBuilding();
            
            _selectedBuilding = null;
            OnSellButtonPress?.Invoke();
            Close(true);
        }

        private void OnUpgradeButton()
        {
            if(!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            ResourceCost[] cost = _selectedBuilding.BuildData().GetUpgradeCost();
            int currentLevel = _selectedBuilding.CurrentLevel() - 1;
            
            for (int i = 0; i < cost[currentLevel].GetCostsAmount(); i++)
                _player.AddResource(cost[currentLevel].GetResource(i).GetKey(), -cost[currentLevel].GetCost(i));

            _selectedBuilding.AddLevel();
            _selectedBuilding.SetCurrentCost(cost[currentLevel]);
            _selectedBuilding.BuildData().Spawn(_selectedBuilding.transform.position, _selectedBuilding.transform, 
                _selectedBuilding.CurrentLevel(), true);
            SoundManager.Instance.Play("Build");
            Close(true);
        }

        private void Open(HexTile tile) {
            if(_open) return;

            _open = true;
            _selectedTile = tile;
            _selectedBuilding = tile.GetCurrentBuilding();
            
            OpenCheck();
            
            OnUiOpen?.Invoke();
            _gameManager.OnUIOpen();
            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if(!_open || _gameManager.GetCurrentTutorial() == Tutorials.BuildTutorial) return;

            Close(true);
        }

        private void Close(bool resumeGame)
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => 
            {
                _viewport.SetActive(false);
                InteractButton.onClick.RemoveAllListeners();
                
                for (int i = 1; i < UpgradeCost.Length; i++)
                    UpgradeCost[i].transform.parent.gameObject.SetActive(false);
                
                if(resumeGame) _gameManager.OnUIClose();
                _open = false;
            };
            
            _selectedTile.DeselectTile();
            _selectedBuilding = null;
            _selectedTile = null;
        }
    }
}
