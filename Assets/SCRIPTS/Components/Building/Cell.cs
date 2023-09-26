using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GGG.Components.Buildings {
    public class Cell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
        [Header("Models")]
        [SerializeField] private GameObject Model;
        [SerializeField] private GameObject SelectedModel;
        [Space(5)]
        [Header("Info")] 
        [SerializeField] private GameObject Spawn;
        
        private BuildingComponent _currentBuilding;
        private bool _empty;

        public Action OnCellClick;

        private void Start()
        {
            _empty = _currentBuilding == null;
        }

        #region Getters & Setters

        public bool IsEmpty() { return _empty; }
        public BuildingComponent GetCurrentBuilding() { return _currentBuilding; }
        public Vector3 SpawnPosition() { return Spawn.transform.position; }
        
        public void SetBuilding(BuildingComponent building) {
            _currentBuilding = building;
            _empty = building == null;
        }

        #endregion

        #region Event Systems Methods

        public void OnPointerDown(PointerEventData eventData) {
            OnCellClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Model.SetActive(false);
            SelectedModel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Model.SetActive(true);
            SelectedModel.SetActive(false);
        }
        
        #endregion
        
    }
}
