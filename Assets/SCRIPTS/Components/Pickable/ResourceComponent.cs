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
        [SerializeField] private int RecollectionTime;
        private bool WaitSecond = true;

        private bool _collided = false;
        private bool _pickingResource;

        public HexTile currentTile;
        public Action onResourceCollideExit;

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

            /*
            WaitSecond = true;
            StartCoroutine(WaitSeconds(0, 3, () =>
                {
                    _pickProgressUI.current++;
                }, () =>
                {
                    _pickProgressUI.current = _pickProgressUI.maximum;
                    RecolectResource();
                    _pickProgressUI.current = 0;
                    _pickProgressUI.gameObject.SetActive(false);
                    _resourcePicked = true;
                    DeleteMySelf();
                }));
                */
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_collided) return;
            
            _pickProgressUI.gameObject.SetActive(false);
            
            if (_pickingResource)
            {
                StopCoroutine(RecollectResource());
                _pickProgressUI.StopPicking();
                _pickingResource = false;
            }
            _collided = false;
        }

        private void Start()
        {
            _pickProgressUI = FindObjectOfType<PickResourceProgressionUI>(true);
            onResourceCollideExit += DeleteMySelf;
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        private IEnumerator RecollectResource()
        {
            _pickingResource = true;
            yield return _pickProgressUI.PickResource(RecollectionTime);
            
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource, out int aux);
            ResourceManager.Instance.resourcesCollected[_resource] += aux;
            _resource.DiscoverResource();
            _pickProgressUI.gameObject.SetActive(false);
            _pickingResource = false;
            DeleteMySelf();
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

        private IEnumerator WaitSeconds(int currentSeconds, int maxSeconds, Action onEachSecond, Action onEnd)
        {

            currentSeconds++;
            yield return new WaitForSeconds(1);
            onEachSecond.Invoke();
            if (!WaitSecond) { yield break; }
            if (currentSeconds < maxSeconds)
            {
                StartCoroutine(WaitSeconds(currentSeconds, maxSeconds, onEachSecond, onEnd));
            }
            else
            {
                onEnd.Invoke();
            }
        }
    }
}