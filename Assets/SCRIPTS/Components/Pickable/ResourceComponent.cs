using GGG.Components.HexagonalGrid;
using GGG.Components.Ticks;
using GGG.Components.UI;
using GGG.Shared;

using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Resources
{
    public class ResourceComponent : MonoBehaviour
    {
        [SerializeField] private Resource _resource;
        [SerializeField] private int _amount;
        [SerializeField] private bool _alwaysVisible;
        [SerializeField] private PickResourceProgressionUI _pickProgressUI;
        [SerializeField] private int _recollectionTime;

        private bool _collided = false;
        private bool _pickingResource;

        public HexTile currentTile;

        #region getters and setters

        public Resource GetResource()
        { return _resource; }

        public int GetAmount()
        { return _amount; }

        public void SetResource(Resource resource)
        { _resource = resource; }

        public void SetAmount(int amount)
        { _amount = amount; }

        #endregion getters and setters

        #region Methods

        private void OnTriggerEnter(Collider other)
        {
            _collided = true;
            _pickProgressUI.gameObject.SetActive(true);

            StartCoroutine(RecollectResource());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_collided) return;
            
            _pickProgressUI.gameObject.SetActive(false);
            
            if (_pickingResource)
            {
                _pickProgressUI.StopPicking();
                _pickingResource = false;
            }
            _collided = false;
        }

        private void Start()
        {
            _pickProgressUI = FindObjectOfType<PickResourceProgressionUI>(true);
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        private void Update()
        {
            if(_pickProgressUI.endedRecollection && _collided)
            {
                _pickProgressUI.endedRecollection = false;

                ResourceManager.Instance.resourcesCollected.TryGetValue(_resource, out int aux);
                ResourceManager.Instance.resourcesCollected[_resource] += aux;
                _resource.DiscoverResource();
                _pickProgressUI.StopPicking();
                _pickProgressUI.gameObject.SetActive(false);
                _pickingResource = false;
                DeleteMySelf();
            }
        }

        private IEnumerator RecollectResource()
        {
            _pickingResource = true;
            yield return null;

            _pickProgressUI.recollectionTime = _recollectionTime;
            _pickProgressUI.ableToPick = true;
        }

        private void DeleteMySelf()
        {
            try
            {
                ResourceManager.Instance.resourcesOnScene.Remove(this);
            }
            catch { }

            ResourceManager.Instance.sumResourceCollected();
            TickManager.OnTick -= HandleVisibility;

            Destroy(gameObject);
        }

        #endregion Methods

        private void HandleVisibility()
        {
            if (!_alwaysVisible)
            {
                if (currentTile != null && currentTile.gameObject.layer == 7)
                {
                    gameObject.layer = 7;
                }
                else if (currentTile != null)
                {
                    gameObject.layer = 9;
                }
            }
        }
    }
}