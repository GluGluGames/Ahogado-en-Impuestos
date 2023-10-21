using GGG.Shared;
using GGG.Components.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;

namespace GGG.Components.Resources
{
    public class ResourceComponent : MonoBehaviour
    {
        [SerializeField] private Resource _resource;
        [SerializeField] private int _amount;
        [SerializeField] private Collider collider;

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
            Destroy(this.gameObject);
        }

        #endregion

    }
}

