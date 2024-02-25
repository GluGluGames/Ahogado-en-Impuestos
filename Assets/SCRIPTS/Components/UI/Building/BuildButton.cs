using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Components.HexagonalGrid;
using GGG.Shared;

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using GGG.Components.Serialization;

namespace GGG.Components.UI.Buildings {
    public class BuildButton : MonoBehaviour, IPointerDownHandler {
        [Header("Information")]
        [SerializeField] private Building BuildingInfo;
        
        private PlayerManager _player;
        private BuildingManager _buildingManager;

        public Action<BuildingComponent, HexTile> OnStructureBuild;
        public Action StructureBuild;

        public void Start()
        {
            _player = PlayerManager.Instance;
            _buildingManager = BuildingManager.Instance;
        }
        
        public Building Building() => BuildingInfo; 

        private void BuildStructure()
        {
            ResourceCost cost = _buildingManager.GetBuildingCost(BuildingInfo);
            HexTile tile = BuildingListener.SelectedTile();
            
            for (int i = 0; i < cost.GetCostsAmount(); i++)
                if (_player.GetResourceCount(cost.GetResource(i).GetKey()) < cost.GetCost(i)) return;
            
            SoundManager.Instance.Play("Build");
            GameObject auxGo = BuildingInfo.Spawn(tile.SpawnPosition(), GameObject.Find("Buildings").transform, 1, false);
            
            for (int i = 0; i < cost.GetCostsAmount(); i++)
            {
                if(!cost.GetResource(i)) continue;
                
                _player.AddResource(cost.GetResource(i).GetKey(), -cost.GetCost(i));
            }

            BuildingComponent build = auxGo.GetComponent<BuildingComponent>();
            build.SetCurrentCost(cost);
            _buildingManager.AddBuilding(build);
            
            tile.SetBuilding(build);
            OnStructureBuild?.Invoke(build, tile);
            StructureBuild?.Invoke();
            
            tile.Reveal(build.VisionRange(), 0);
            SerializationManager.Instance.Save();
        }

        #region Event Systems Method

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BuildingInfo.CanBeBoost() && GameManager.Instance.OnTutorial()) return;
            
            if (!BuildingListener.CanBuild(BuildingInfo) ||
                !BuildingListener.SelectedTile().TileEmpty() ||
                GameManager.Instance.TutorialOpen()) return;
            
            BuildStructure();
        }

        #endregion

    }
    
}
