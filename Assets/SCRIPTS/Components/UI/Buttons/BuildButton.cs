using DG.Tweening;
using GGG.Classes.Buildings;
using TMPro;
using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.UI {
    public class BuildButton : MonoBehaviour, IPointerDownHandler {
        [SerializeField] private Building BuildingInfo;
        [SerializeField] private GameObject Container;
        [SerializeField] private GameObject[] ResourcesContainers;
        [SerializeField] private Image[] ResourcesImages;
        [SerializeField] private Image StructureSprite;
        [SerializeField] private GameObject Padlock;
        
        private PlayerManager _player;
        private HexTile _selectedHexTile;
        private BuildingComponent _auxBuild;
        private ResourceCost _cost;
        private bool _dirtyFlag;

        public Action<BuildingComponent, HexTile> OnStructureBuild;
        public Action StructureBuild;

        public void Initialize()
        {
            _player = PlayerManager.Instance;
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }
            
            TextMeshProUGUI[] texts = Container.GetComponentsInChildren<TextMeshProUGUI>();
            
            _cost = BuildingInfo.GetBuildingCost();

            StructureSprite.sprite = BuildingInfo.GetIcon();
            ResourcesContainers[0].SetActive(true);
            texts[0].text = _cost.GetCost(0).ToString();
            ResourcesImages[0].sprite = _cost.GetResource(0).GetSprite();
            
            if (!BuildingInfo.IsUnlocked())
            {
                StructureSprite.color = new Color(1, 1, 1, 0.5f);
                ResourcesImages[0].gameObject.SetActive(false);
                texts[0].gameObject.SetActive(false);
                Padlock.gameObject.SetActive(true);
                return;
            }

            for (int i = 1; i < ResourcesImages.Length; i++)
            {
                if (i >= _cost.GetCostsAmount() || _cost.GetCost(i) == 0)
                    continue;
                
                ResourcesContainers[i].SetActive(true);
                texts[i].SetText(_cost.GetCost(i).ToString());
                ResourcesImages[i].sprite = _cost.GetResource(i).GetSprite();
            }
            
        }

        public void CheckUnlockState()
        {
            if (!BuildingInfo.IsUnlocked() || _dirtyFlag) return;
            
            StructureSprite.color = new Color(1, 1, 1, 1);
            ResourcesImages[0].gameObject.SetActive(true);
            Padlock.gameObject.SetActive(false);
            
            TextMeshProUGUI[] texts = Container.GetComponentsInChildren<TextMeshProUGUI>();
            
            for (int i = 1; i < ResourcesImages.Length; i++)
            {
                if (i >= _cost.GetCostsAmount() || _cost.GetCost(i) == 0)
                    continue;
                
                ResourcesImages[i].gameObject.SetActive(true);
                texts[i].gameObject.SetActive(true);
                texts[i].SetText(_cost.GetCost(i).ToString());
                ResourcesImages[i].sprite = _cost.GetResource(i).GetSprite();
            }

            _dirtyFlag = true;
        }

        private void BuildStructure()
        {
            for (int i = 0; i < _cost.GetCostsAmount(); i++)
            {
                if (_player.GetResourceCount(_cost.GetResource(i).GetKey()) < _cost.GetCost(i))
                {
                    // TODO - Can't buy warning
                    return;
                }
            }
            
            SoundManager.Instance.Play("Build");
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition(), GameObject.Find("Buildings").transform, 1, false);
            _auxBuild = auxGo.GetComponent<BuildingComponent>();
            BuildingManager.Instance.AddBuilding(_auxBuild);

            _selectedHexTile.SetBuilding(_auxBuild);
            OnStructureBuild?.Invoke(_auxBuild, _selectedHexTile);
            StructureBuild?.Invoke();

            for (int i = 0; i < _cost.GetCostsAmount(); i++)
            {
                if(!_cost.GetResource(i)) continue;
                
                _player.AddResource(_cost.GetResource(i).GetKey(), -_cost.GetCost(i));
            }


            //FOW
            _selectedHexTile.Reveal(_auxBuild.GetVisionRange(), 0);

            _selectedHexTile = null;
        }

        private void UnlockBuilding()
        {
            Padlock.gameObject.SetActive(false);
        }

        #region Event Systems Method

        public void OnPointerDown(PointerEventData eventData) {
            if (!BuildingInfo.IsUnlocked() || !_selectedHexTile.TileEmpty() || GameManager.Instance.TutorialOpen()) return;
            
            BuildStructure();
        }

        #endregion

    }
    
}
