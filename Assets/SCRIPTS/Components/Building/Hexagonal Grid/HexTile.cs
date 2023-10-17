using GGG.Shared;

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings {
    public class HexTile : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler {
        public HexTileGenerationSettings settings;
        public HexTileGenerationSettings.TileType tileType;

        public GameObject tilePrefab;
        public GameObject fow;
        public Vector2Int offsetCoordinate;
        public Vector3Int cubeCoordinate;
        public List<HexTile> neighbours;

        private TileManager _manager;
        private GameObject _highlightPrefab;
        private BuildingComponent _currentBuilding;

        private bool _isDirty = false;
        private bool _isEmpty;
        private bool _selected = false;

        public Action OnHexHighlight;
        public Action OnHexSelect;

        #region Unity Events

        private void OnValidate() {
            if (tilePrefab == null) { return; }
            _isDirty = true;
        }

        private void Start() {
            _manager = TileManager.instance;
            _isEmpty = _currentBuilding == null;

            if (_manager)
            {
                _highlightPrefab = Instantiate(_manager.highlightPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
                _highlightPrefab.SetActive(false);
            }
        }

        private void Update() {
            if (_isDirty) {

                if (Application.isPlaying) {
                    GameObject.Destroy(tilePrefab);
                } else {
                    GameObject.DestroyImmediate(tilePrefab);
                }
                tilePrefab = null;

                AddTile();
                _isDirty = false;
            }
        }

        #endregion

        #region Getters&Setters

        public bool TileEmpty() { return _isEmpty; }
        public BuildingComponent GetCurrentBuilding() { return _currentBuilding; }
        public Vector3 SpawnPosition() { return transform.position + new Vector3(0, 1f); }
        public void SetBuilding(BuildingComponent building) {
            _currentBuilding = building;
            _isEmpty = building == null;
        }

        #endregion

        #region Methods

        // summary:
        //  Generate random type of tile
        public void RollTileType() {
            tileType = (HexTileGenerationSettings.TileType)Random.Range(0, 3);
        }

        public void AddTile() {
            tilePrefab = Instantiate(settings.GetTile(tileType), transform.position, Quaternion.Euler(0f, 0f, 0f), transform);

            if (gameObject.GetComponent<MeshCollider>() == null) {
                MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
            }
        }

        private void SelectTile() {
            if (_manager.GetSelectedTile()) {
                _manager.GetSelectedTile()._selected = false;
                _manager.GetSelectedTile().DeactivateHighlight();
            }

            _selected = true;
            _manager.SelectTile(this);
        }

        public void DeselectTile() {
            if(!_selected) return;

            _manager.SelectTile(null);
            _selected = false;
            DeactivateHighlight();
        }

        private void ActivateHighlight() {
            tilePrefab.SetActive(false);
            _highlightPrefab.SetActive(true);
        }
        
        private void DeactivateHighlight() {
            tilePrefab.SetActive(true);
            _highlightPrefab.SetActive(false);
        }

        private IEnumerator TouchWait()
        {
            yield return new WaitForSeconds(0.1f);
            if (Holding.IsHolding()) yield break;

            if (_currentBuilding) {
                _currentBuilding.Interact();
                yield break;
            }

            ActivateHighlight();
            SelectTile();
            OnHexSelect?.Invoke();
            
        }

        #endregion

        #region Event System Methods
        public void OnPointerEnter(PointerEventData eventData) {
            ActivateHighlight();
            OnHexHighlight?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData) {
            if(_selected) return;

            DeactivateHighlight();
        }

        public void OnPointerDown(PointerEventData eventData) {
            StartCoroutine(TouchWait());
        }

        /// <summary>
        /// Reveals tiles around itself, recursive function. Range is dependant on <paramref name="depth"/>
        /// </summary>
        /// <param name="depth"> Range of the tiles to clear </param> 
        /// <param name="iter"> Internal stop variable, set it to 0 on base call </param>
        public void Reveal(int depth, int iter)
        {
            if(iter > depth) return;
            ++iter;
            foreach(HexTile neighbour in neighbours)
            {
                neighbour.Reveal(depth, iter);
            }
            gameObject.layer = 0;
            transform.GetChild(0).gameObject.layer = 0;
            fow.SetActive(false);
        }

        #endregion
    }
}


