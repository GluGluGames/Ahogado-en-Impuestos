using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using GGG.Components.Core;
using GGG.Components.UI.Buttons;
using GGG.Shared;
using GGG.Input;

using System;
using TMPro;
using DG.Tweening;
using GGG.Components.Buildings.CityHall;
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
        [SerializeField] private Sound SellSound;
        [Space(5),Header("Upgrade Fields")]
        [SerializeField] private Button UpgradeButton;
        [SerializeField] private Image[] UpgradeResources;
        [SerializeField] private TMP_Text[] UpgradeCost;
        [Space(5), Header("Other fields")]
        [SerializeField] private Button InteractButton;
        [SerializeField] private Button CloseButton;

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
        public Action OnCloseButtonPress;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            
            Initialize();
        }

        private void Update()
        {
            if (!_open || !_input.Escape() || _gameManager.OnTutorial()) return;
            
            Close(true); 
        }

        private void OnDisable()
        {
            _buttons = FindObjectsOfType<BuildButton>(true);
            
            foreach (BuildButton button in _buttons) 
                button.OnStructureBuild -= UpdateBuildings;

            TileManager.OnBuildingTileLoaded -= UpdateBuildings;
            _player.OnPlayerInitialized -= SetSellResource;
            
            SellButton.onClick.RemoveAllListeners();
            UpgradeButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
        }

        private void Initialize()
        {
            _buttons = FindObjectsOfType<BuildButton>(true);
            
            foreach (BuildButton button in _buttons) 
                button.OnStructureBuild += UpdateBuildings;

            foreach (Image upgrade in UpgradeResources)
                upgrade.transform.parent.gameObject.SetActive(false);
            
            foreach (TMP_Text text in UpgradeCost)
                text.gameObject.SetActive(false);
                    
            SellResource.transform.parent.gameObject.SetActive(false);
            
            TileManager.OnBuildingTileLoaded += UpdateBuildings;
            _player.OnPlayerInitialized += SetSellResource;

            SellButton.onClick.AddListener(OnSellButton);
            UpgradeButton.onClick.AddListener(OnUpgradeButton);
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void SetSellResource() => _sellResource = _player.GetResource("Seaweed");
        
        private void OpenCheck()
        {
            int currentLevel = _selectedBuilding.CurrentLevel();
            ResourceCost[] cost = _selectedBuilding.BuildData().GetUpgradeCost();
            bool price = true;
            bool sell = _selectedBuilding.GetType() != typeof(CityHall);

            SellButton.interactable = sell;
            SellButton.image.color = sell ? Color.white : new Color(0.81f, 0.81f, 0.81f);
            
            SellResource.transform.parent.gameObject.SetActive(sell);
            SellResource.gameObject.SetActive(sell);
            SellCost.gameObject.SetActive(sell);
            
            SellResource.sprite = _selectedBuilding.BuildData().GetBuildResource(0).GetSprite();
            SellCost.SetText(Mathf.RoundToInt(_selectedBuilding.CurrentCost().GetCost(0) * 0.5f).ToString());

            bool activeCondition = _selectedBuilding.BuildData().CanUpgraded() &&
                                   currentLevel < _selectedBuilding.BuildData().GetMaxLevel();
            
            UpgradeButton.interactable = activeCondition;
            UpgradeButton.image.color = activeCondition ? Color.white : new Color(0.81f, 0.81f, 0.81f);
            foreach (TMP_Text upgradeCost in UpgradeCost)
                upgradeCost.transform.parent.gameObject.SetActive(activeCondition);

            if (!activeCondition) return;
            
            for (int i = 0; i < UpgradeCost.Length; i++)
            {
                if (!_selectedBuilding.BuildData().GetUpgradeResource(currentLevel, i))
                {
                    UpgradeCost[i].transform.parent.gameObject.SetActive(false);
                    break;
                }
                
                UpgradeCost[i].transform.parent.gameObject.SetActive(true);
                UpgradeCost[i].gameObject.SetActive(true);
                UpgradeCost[i].text = _selectedBuilding.BuildData().GetUpgradeCost(currentLevel, i).ToString();
                UpgradeResources[i].sprite = _selectedBuilding.BuildData().GetUpgradeResource(currentLevel, i).GetSprite();
                
                if (_player.GetResourceCount(cost[currentLevel - 1].GetResource(i).GetKey()) >= cost[currentLevel - 1].GetCost(i)) continue;
                
                price = false;
            }

            UpgradeButton.interactable = price;
            UpgradeButton.image.color = price ? Color.white : new Color(0.81f, 0.81f, 0.81f, 0.9f);
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
        }

        private void OnSellButton()
        {
            if(!_open || _gameManager.TutorialOpen()) return;
            
            _player.AddResource(_sellResource.GetKey(), Mathf.RoundToInt(_selectedBuilding.CurrentCost().GetCost(0) * 0.5f));
            _selectedBuilding.OnBuildDestroy();
            BuildingManager.Instance.RemoveBuilding(_selectedBuilding);
            SoundManager.Instance.Play(SellSound);
            _selectedTile.DestroyBuilding();
            
            _selectedBuilding = null;
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
            BuildingManager.Instance.SaveBuildings();
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
            if(!_open || _gameManager.TutorialOpen()) return;

            OnCloseButtonPress?.Invoke();
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
