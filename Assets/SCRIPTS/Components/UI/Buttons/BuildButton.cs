using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Components.HexagonalGrid;
using GGG.Shared;

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;

namespace GGG.Components.UI.Buttons {
    public class BuildButton : MonoBehaviour, IPointerDownHandler {
        [Header("Information")]
        [SerializeField] private Building BuildingInfo;
        [Space(5), Header("GameObjects References")]
        [SerializeField] private GameObject Container;
        [SerializeField] private GameObject[] ResourcesContainers;
        [SerializeField] private GameObject Padlock;
        [Space(5), Header("Texts")] 
        [SerializeField] private TMP_Text StructureName;
        [Space(5), Header("Images")]
        [SerializeField] private Image[] ResourcesImages;
        [SerializeField] private Image StructureSprite;
        
        private PlayerManager _player;
        private BuildingManager _buildingManager;
        private HexTile _selectedHexTile;
        private ResourceCost _cost;
        private bool _dirtyFlag;
        private bool _maxBuildingsReached;

        public Action<BuildingComponent, HexTile> OnStructureBuild;
        public Action StructureBuild;

        public void Initialize(BuildingManager manager)
        {
            _player = PlayerManager.Instance;
            _buildingManager = manager;
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }
            
            _cost = _buildingManager.GetBuildingCost(BuildingInfo);
            StructureSprite.sprite = BuildingInfo.GetIcon();
            StructureName.SetText(BuildingInfo.GetName());
            
            if (!BuildingInfo.IsUnlocked()) LockButton();
        }
        

        private void LockButton()
        {
            StructureSprite.color = new Color(1, 1, 1, 0.5f);
            ResourcesContainers[0].SetActive(false);
            Padlock.gameObject.SetActive(true);
            
            for (int i = 1; i < ResourcesContainers.Length; i++)
            {
                if (!ResourcesContainers[i].gameObject.activeInHierarchy)
                    continue;
                
                ResourcesContainers[i].SetActive(false);
            }
        }

        public void CheckUnlockState()
        {
            StructureName.SetText(BuildingInfo.GetName());
            int cont = _buildingManager.GetBuildCount(BuildingInfo);
            TextMeshProUGUI[] texts = Container.GetComponentsInChildren<TextMeshProUGUI>(true);

            if (!BuildingInfo.IsUnlocked() 
                || (BuildingInfo.GetMaxBuildingNumber() != -1 && cont >= BuildingInfo.GetMaxBuildingNumber()))
            {
                LockButton();
                _maxBuildingsReached = true;
                return;
            }
            
            Padlock.gameObject.SetActive(false);
            StructureSprite.color = new Color(1, 1, 1, 1);
            ResourcesContainers[0].SetActive(true);
            _maxBuildingsReached = false;
            
            _cost = _buildingManager.GetBuildingCost(BuildingInfo);
            texts[0].text = _cost.GetCost(0).ToString();
            ResourcesImages[0].sprite = _cost.GetResource(0).GetSprite();
            
            for (int i = 1; i < ResourcesImages.Length; i++)
            {
                if (i >= _cost.GetCostsAmount() || _cost.GetCost(i) == 0)
                    continue;
                
                ResourcesContainers[i].gameObject.SetActive(true);
                texts[i].SetText(_cost.GetCost(i).ToString());
                ResourcesImages[i].sprite = _cost.GetResource(i).GetSprite();
            }
        }

        private void BuildStructure()
        {
            for (int i = 0; i < _cost.GetCostsAmount(); i++)
                if (_player.GetResourceCount(_cost.GetResource(i).GetKey()) < _cost.GetCost(i)) return;
            
            SoundManager.Instance.Play("Build");
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition(), 
                GameObject.Find("Buildings").transform, 1, false);
            
            for (int i = 0; i < _cost.GetCostsAmount(); i++)
            {
                if(!_cost.GetResource(i)) continue;
                
                _player.AddResource(_cost.GetResource(i).GetKey(), -_cost.GetCost(i));
            }

            BuildingComponent build = auxGo.GetComponent<BuildingComponent>();
            _buildingManager.AddBuilding(build);
            build.SetCurrentCost(_cost);
            
            _selectedHexTile.SetBuilding(build);
            OnStructureBuild?.Invoke(build, _selectedHexTile);
            StructureBuild?.Invoke();

            //FOW
            _selectedHexTile.Reveal(build.VisionRange(), 0);
            _selectedHexTile = null;
        }

        #region Event Systems Method

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BuildingInfo.CanBeBoost() && GameManager.Instance.GetCurrentTutorial() is Tutorials.BuildTutorial)
                return;
            
            if (!BuildingInfo.IsUnlocked() || _maxBuildingsReached
                || !_selectedHexTile.TileEmpty() || GameManager.Instance.TutorialOpen()) return;
            
            BuildStructure();
        }

        #endregion

    }
    
}
