using DG.Tweening;
using GGG.Classes.Buildings;
using TMPro;
using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.EventSystems;

namespace GGG.Components.UI {
    public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
        [SerializeField] private Building BuildingInfo;
        [SerializeField] private TextMeshProUGUI BuildNameText;

        private Cell _selectedCell;
        private BuildingComponent _auxBuild;

        private void Start() {
            Cell[] cells = FindObjectsOfType<Cell>();

            foreach (Cell cell in cells)
                cell.OnCellClick += (building) => {
                    _selectedCell = cell;

                    if (!_selectedCell.IsEmpty()) _auxBuild = building;
                };
             
            BuildNameText.SetText(BuildingInfo.GetName());
        }

        private void BuildStructure() {
            GameObject auxGo = BuildingInfo.Spawn(_selectedCell.SpawnPosition());
            _selectedCell.SetBuilding(auxGo.GetComponent<BuildingComponent>());
            
            print("Structure built");
        }

        #region Event Systems Method
        
        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        }

        #endregion

        public void OnPointerDown(PointerEventData eventData) {
            if (!_selectedCell.IsEmpty()) return;
            
            BuildStructure();
        }
    }
    
}
