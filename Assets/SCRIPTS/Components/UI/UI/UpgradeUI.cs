using GGG.Components.Buildings;
using GGG.Components.Player;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using GGG.Components.Core;
using GGG.Shared;
using System.Collections.Generic;
using System.Globalization;
using TMPro;

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

        private void Update()
        {
            if (!_selectedBuilding) return;
            if (!_selectedBuilding.GetBuild().CanUpgraded())
            {
                UpgradePanel.gameObject.SetActive(false);
                return;
            }
            
            UpgradePanel.gameObject.SetActive(true);
            ResourceCost[] cost = _selectedBuilding.GetBuild().GetUpgradeCost();
            int currentLevel = _selectedBuilding.GetCurrentLevel() - 1;
            
            for (int i = 0; i < cost[currentLevel].GetCostsAmount(); i++)
            {
                if (_player.GetResourceCount(cost[currentLevel].GetResource(i).GetKey()) <
                    cost[currentLevel].GetCost(i))
                {
                    UpgradeButton.interactable = false;
                    UpgradeButton.image.color = new Color(0.81f, 0.84f, 0.81f, 0.9f);
                    return;
                }
            }

            UpgradeButton.interactable = true;
            UpgradeButton.image.color = new Color(1f, 1f, 1f, 1f);
        }

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
            _player.AddResource(_sellResource.GetKey(), Mathf.RoundToInt(_selectedBuilding.GetBuild().GetBuildingCost(0) * 0.5f));
            _selectedTile.DestroyBuilding();
            
            _selectedBuilding = null;
            Close(true);
        }

        private void OnUpgradeButton()
        {
            ResourceCost[] cost = _selectedBuilding.GetBuild().GetUpgradeCost();
            int currentLevel = _selectedBuilding.GetCurrentLevel() - 1;

            for (int i = 0; i < cost[currentLevel].GetCostsAmount(); i++)
            {
                if (_player.GetResourceCount(cost[currentLevel].GetResource(i).GetKey()) <
                    cost[currentLevel].GetCost(i))
                {
                    // TODO - Can't upgrade warning
                    return;
                }
            }

            for (int i = 0; i < cost[currentLevel].GetCostsAmount(); i++)
            {
                _player.AddResource(cost[currentLevel].GetResource(i).GetKey(), -cost[currentLevel].GetCost(i));
            }
            
            
            _selectedBuilding.AddLevel();
            _selectedBuilding.GetBuild().Spawn(_selectedBuilding.transform.position, _selectedBuilding.transform, _selectedBuilding.GetCurrentLevel(), true);
            SoundManager.Instance.Play("Build");
            Close(true);
        }

        private void Open(HexTile tile) {
            if(_open) return;

            _open = true;
            _selectedTile = tile;
            _selectedBuilding = tile.GetCurrentBuilding();
            _panel.SetActive(true);
            
            SellResource.sprite = _selectedBuilding.GetBuild().GetBuildResource(0).GetSprite();
            SellCost.SetText(Mathf.RoundToInt(_selectedBuilding.GetBuild().GetBuildingCost(0) * 0.5f).ToString());

            int currentLevel = _selectedBuilding.GetCurrentLevel();
            
            if (currentLevel < _selectedBuilding.GetBuild().GetMaxLevel())
            {
                for (int i = 0; i < UpgradeCost.Length; i++)
                {
                    if (!_selectedBuilding.GetBuild().GetUpgradeResource(currentLevel, i))
                        break;
                
                    UpgradeCost[i].transform.parent.gameObject.SetActive(true);
                
                    UpgradeCost[i].text = _selectedBuilding.GetBuild().GetUpgradeCost(currentLevel, i).ToString();
                    UpgradeResources[i].sprite =
                        _selectedBuilding.GetBuild().GetUpgradeResource(currentLevel, i).GetSprite();
                }
            } 
            else UpgradeButton.gameObject.SetActive(false);

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
                
                for (int i = 1; i < UpgradeCost.Length; i++)
                {
                    UpgradeCost[i].transform.parent.gameObject.SetActive(false);
                }
            };
            
            _selectedTile.DeselectTile();
            _selectedBuilding = null;
            _selectedTile = null;
            if(resumeGame) GameManager.Instance.OnUIClose();
            _open = false;
        }
    }
}
