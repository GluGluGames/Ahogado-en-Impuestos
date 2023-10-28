using GGG.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings
{
    [ExecuteAlways, Serializable]
    public class HexTile : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        public HexTileGenerationSettings settings;
        public TileType tileType;

        public GameObject tilePrefab;
        public GameObject fow;
        public Vector2Int offsetCoordinate;
        public Vector3Int cubeCoordinate;
        public List<HexTile> neighbours;

        [SerializeField] private int ClearCost;

        private TileManager _manager;
        private GameObject _highlightPrefab;
        private BuildingComponent _currentBuilding;

        private bool _isDirty = false;
        private bool _isEmpty;
        private bool _selected = false;

        public Action OnHexHighlight;
        public Action<HexTile> OnHexSelect;
        public Action OnHexDeselect;

        #region Unity Events

        private void OnValidate()
        {
            if (tilePrefab == null || Application.isPlaying) { return; }
            _isDirty = true;
        }

        private void Start()
        {
            _manager = TileManager.instance;
            _isEmpty = _currentBuilding == null;

            if (_manager)
            {
                _highlightPrefab = Instantiate(_manager.highlightPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
                _highlightPrefab.SetActive(false);
            }

            // TODO - Apply the cost manually
            ClearCost = 50;
        }

        private void Update()
        {
            if (_isDirty && Application.isEditor)
            {
                Transform goAux = transform.Find("HighlightTile(Clone)");

                if (Application.isPlaying)
                {
                    GameObject.Destroy(tilePrefab);
                    if (goAux != null)
                        GameObject.Destroy(goAux.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(tilePrefab);
                    if (goAux != null)
                        GameObject.DestroyImmediate(goAux.gameObject);
                }
                tilePrefab = null;

                AddTile();
                _isDirty = false;
            }

            _isEmpty = !_currentBuilding;
        }

        #endregion Unity Events

        #region Getters&Setters

        public bool TileEmpty()
        { return !_currentBuilding; }

        public BuildingComponent GetCurrentBuilding()
        { return _currentBuilding; }

        public Vector3 SpawnPosition()
        { return transform.position + new Vector3(0, 1f); }

        public void SetBuilding(BuildingComponent building)
        {
            _currentBuilding = building;
            if(building) 
                building.SetTile(this);
            _isEmpty = !building;
        }

        public TileType GetTileType()
        { return tileType; }

        public void SetTileType(TileType type)
        {
            tileType = type;
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            AddTile();
        }

        public int GetClearCost()
        { return ClearCost; }

        #endregion Getters&Setters

        #region Methods

        // summary:
        //  Generate random type of tile
        public void RollTileType()
        {
            tileType = (TileType)Random.Range(0, 3);
        }

        public void AddTile()
        {
            tilePrefab = Instantiate(settings.GetTile(tileType), transform.position, Quaternion.Euler(0f, 0f, 0f), transform);

            if (gameObject.GetComponent<MeshCollider>() == null)
            {
                MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
            }

            if (_manager)
            {
                _highlightPrefab = Instantiate(_manager.highlightPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
                _highlightPrefab.SetActive(false);
            }
        }

        private void SelectTile()
        {
            if (_manager.GetSelectedTile() && _manager.GetSelectedTile() != this)
                _manager.GetSelectedTile().DeselectTile();

            _selected = true;
            _manager.SelectTile(this);
            OnHexSelect?.Invoke(this);
        }

        public void DeselectTile()
        {
            if (!_selected) return;

            OnHexDeselect?.Invoke();
            _manager.SelectTile(null);
            _selected = false;
            DeactivateHighlight();
        }

        private void ActivateHighlight()
        {
            tilePrefab.SetActive(false);
            _highlightPrefab.SetActive(true);
        }

        private void DeactivateHighlight()
        {
            tilePrefab.SetActive(true);
            _highlightPrefab.SetActive(false);
        }

        private IEnumerator TouchWait()
        {
            yield return new WaitForSeconds(0.1f);
            if (Holding.IsHolding() || GameManager.Instance.IsOnUI()) yield break;

            if (_currentBuilding)
            {
                _currentBuilding.Interact();
            }

            ActivateHighlight();
            SelectTile();
        }

        public void DestroyBuilding()
        {
            _currentBuilding.SetTile(null);
            Destroy(_currentBuilding.gameObject);
            _currentBuilding = null;
        }

        /// <summary>
        /// Reveals tiles around itself, recursive function. Range is dependant on <paramref name="depth"/>
        /// </summary>
        /// <param name="depth"> Range of the tiles to clear </param>
        /// <param name="iter"> Internal stop variable, set it to 0 on base call </param>
        public void Reveal(int depth, int iter)
        {
            if (iter > depth) return;
            ++iter;
            foreach (HexTile neighbour in neighbours)
            {
                neighbour.Reveal(depth, iter);
            }
            gameObject.layer = 0;
            transform.GetChild(0).gameObject.layer = 0;
            if (fow) fow.SetActive(false);
        }

        /// <summary>
        /// Only used on the minigame. It dicides if a tile is walkable. Used to know if an enemy can spawn. This is prob. useless
        /// </summary>
        public bool Walkable()
        {
            return (_isEmpty);
        }

        #endregion Methods

        #region Event System Methods

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (GameManager.Instance.IsOnUI()) return;
            
            ActivateHighlight();
            OnHexHighlight?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selected) return;

            DeactivateHighlight();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            
            StartCoroutine(TouchWait());
        }

        #endregion Event System Methods
    }
}