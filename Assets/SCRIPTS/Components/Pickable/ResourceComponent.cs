using GGG.Shared;
using GGG.Components.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;
using GGG.Components.Ticks;

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
        public Resource GetResource() { return _resource; }
        public int GetAmount() { return _amount; }

        public void SetResource(Resource resource) { _resource = resource; }
        public void SetAmount(int amount) { _amount = amount; }

        #endregion

        #region Methods

        private void OnTriggerEnter(Collider other)
        {
            //if(!_collided)
            //{
            //    onResourceCollideEnter.Invoke();
            //    _collided = true;
            //}
            RecolectResource();
            DeleteMySelf();
        }

        private void OnTriggerExit(Collider other)
        {
            if(_collided)
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
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource.GetKey(), out aux);
            Debug.Log("prev value: " + aux);

            ResourceManager.Instance.resourcesCollected.Remove(_resource.GetKey());
            ResourceManager.Instance.resourcesCollected.Add(_resource.GetKey(), _amount + aux);

            int debug = 0;
            ResourceManager.Instance.resourcesCollected.TryGetValue(_resource.GetKey(), out debug);
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

        #endregion

        private void HandleVisibility()
        {
            if(!_alwaysVisible)
            {
                if(currentTile != null && currentTile.gameObject.layer == 7)
                {
                    gameObject.layer = 7;
                }
                else if(currentTile != null)
                {
                    gameObject.layer = 9;
                }
            }
        }
    }
}

