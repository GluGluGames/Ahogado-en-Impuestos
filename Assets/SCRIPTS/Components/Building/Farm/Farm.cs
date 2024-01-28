using System;
using GGG.Components.Player;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class Farm : BuildingComponent
    {
        [Tooltip("Type of the farm")] 
        [SerializeField] private FarmTypes FarmType;
        [Tooltip("Time of resource generation without upgrades")]
        [SerializeField] private float InitialGeneration;
        [Tooltip("Time of resource generation with the upgrades")] 
        [SerializeField] private float[] ResourcesGeneration;

        private PlayerManager _playerManager;
        private FarmUI _ui;
        private Resource _generationResource;
        private GameObject _resourceModel;
        
        private float _currentGeneration;
        private float _cooldownDelta;
        
        public override void Initialize()
        {
            _playerManager = PlayerManager.Instance;
            if (!_ui) _ui = FindObjectOfType<FarmUI>();
            _currentGeneration = _currentLevel == 1 ? InitialGeneration : ResourcesGeneration[_currentLevel - 2];
        }

        private void Update()
        {
            if (_generationResource) Produce();
            
            if (!ResourceModel()) return;
            
            _resourceModel.transform.Rotate(Vector3.up * (Time.deltaTime * 25f));
        }

        public override void Interact()
        {
            _ui.Open(this);
        }

        protected override void OnLevelUp()
        {
            _currentGeneration = ResourcesGeneration[_currentLevel - 2];
            if (_boosted) Boost();
        }

        public override void OnBuildDestroy()
        {
            _generationResource = null;
            _resourceModel = null;
        }

        public override void Boost()
        {
            base.Boost();
            if (_currentLevel == 1) _currentGeneration -= InitialGeneration * 0.25f;
            else _currentGeneration -= ResourcesGeneration[_currentLevel - 2] * 0.25f;

            _boosted = true;
        }

        public override void EndBoost()
        {
            base.EndBoost();
            if (_currentLevel == 1) _currentGeneration += InitialGeneration * 0.25f;
            else _currentGeneration += ResourcesGeneration[_currentLevel - 2] * 0.25f;

            _boosted = false;
        }

        private void Produce()
        {
            if(!_generationResource) return;
            
            _cooldownDelta -= Time.deltaTime;
            
            if (_cooldownDelta >= 0f) return;
            
            _playerManager.AddResource(_generationResource.GetKey(), 1);
            _cooldownDelta = _currentGeneration;
        }
        
        public Resource GetResource() => _generationResource;
        public void Resource(Resource resource) => _generationResource = resource;
        public GameObject ResourceModel() => _resourceModel;
        public void SetResourceModel(Resource resource)
        {
            if (_resourceModel) Destroy(_resourceModel);

            _resourceModel =
                Instantiate(resource.GetModel(), transform.position + Vector3.up * 3, Quaternion.identity, transform);
            _resourceModel.transform.localScale = resource.GetModelScale();
        }
        public float GetGeneration() => _currentGeneration;
        public FarmTypes Type() => FarmType;

    }
}