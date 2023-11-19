using GGG.Components.Buildings;
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
        private bool WaitSecond = true;

        private bool _collided = false;

        public HexTile currentTile;
        public Action onResourceCollideEnter;
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
            WaitSecond = true;
            Coroutine aux = StartCoroutine(
                WaitSeconds(0, 3, 
                () => 
                {
                    _pickProgressUI.current++;
                },
                () =>
                {
                    _pickProgressUI.current = _pickProgressUI.maximum;
                    RecolectResource();
                    _pickProgressUI.current = 0;
                    _pickProgressUI.gameObject.SetActive(false);
                    DeleteMySelf();
                }));
        }

        private void OnTriggerExit(Collider other)
        {

            WaitSecond = false;
            if (_collided)
            {
                _pickProgressUI.current = 0;
                _pickProgressUI.gameObject.SetActive(false);
                onResourceCollideExit.Invoke();
                _collided = false;
            }
        }

        private void Start()
        {
            _pickProgressUI = GameObject.FindGameObjectWithTag("Player").transform.Find("HeaderBar").Find("ProgressBar").GetComponent<PickResourceProgressionUI>();
            onResourceCollideEnter += RecolectResource;
            onResourceCollideExit += DeleteMySelf;
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        private void RecolectResource()
        {
            int aux = 0;
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource.GetName(), out aux);

            ResourceManager.Instance.resourcesCollected.Remove(_resource.GetName());
            ResourceManager.Instance.resourcesCollected.Add(_resource.GetName(), _amount + aux);
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

            Destroy(this.gameObject);
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