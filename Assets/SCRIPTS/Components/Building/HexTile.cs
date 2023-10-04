using System;
using System.Collections.Generic;
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
        public Vector3 SpawnPosition() { return transform.position + new Vector3(0, 0.5f); }
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
            tilePrefab = Instantiate(settings.GetTile(tileType), transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
            _highlightPrefab = Instantiate(_manager.highlightPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
            _highlightPrefab.SetActive(false);

            if (gameObject.GetComponent<MeshCollider>() == null) {
                MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
            }
        }

        #endregion

        #region Event System Methods
        public void OnPointerEnter(PointerEventData eventData) {
            tilePrefab.SetActive(false);
            _highlightPrefab.SetActive(true);
            OnHexHighlight?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData) {
            tilePrefab.SetActive(true);
            _highlightPrefab.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData) {
            OnHexSelect?.Invoke();

        }
        #endregion
        
        /*
        public void OnDrawGizmosSelected() {
            foreach (HexTile neighbour in neighbours) {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(transform.position, 0.2f);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }
        }
        */

        /*
        public void OnHighlightTile()
        {
            Debug.Log(TileManager.instance);
            TileManager.instance.OnHighlightTile(this);
        }

        public void OnSelectTile()
        {
            TileManager.instance.OnSelectTile(this);

        }
        */
    }
}


