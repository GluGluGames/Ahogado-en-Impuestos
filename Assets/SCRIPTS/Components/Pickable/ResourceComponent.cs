using GGG.Components.Buildings;
using GGG.Components.Ticks;
using GGG.Shared;
using System;
using UnityEngine;

namespace GGG.Components.Resources
{
    public class ResourceComponent : MonoBehaviour
    {
        [SerializeField] private Resource _resource;
        [SerializeField] private int _amount;
        [SerializeField] private Collider colliderResource;
        [SerializeField] private bool _alwaysVisible;

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
            StartCoroutine(TickManager.Instance.WaitSeconds(0, 3, () => Debug.Log("esperando..."),
                () =>
                {
                    Debug.Log("FIN!!!");
                    RecolectResource();
                    DeleteMySelf();
                }));

        }

        private void OnTriggerExit(Collider other)
        {
            if (_collided)
            {
                onResourceCollideExit.Invoke();
                _collided = false;
            }
        }

        private void Start()
        {
            onResourceCollideEnter += RecolectResource;
            onResourceCollideExit += DeleteMySelf;
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        private void RecolectResource()
        {
            int aux = 0;
            Debug.Log(ResourceManager.Instance);
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource.GetName(), out aux);
            Debug.Log("prev value: " + aux);

            ResourceManager.Instance.resourcesCollected.Remove(_resource.GetName());
            ResourceManager.Instance.resourcesCollected.Add(_resource.GetName(), _amount + aux);

            int debug = 0;
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource.GetName(), out debug);
            Debug.Log("post value: " + debug);
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
    }
}