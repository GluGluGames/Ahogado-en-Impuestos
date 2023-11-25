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
        
        private float _currentGeneration;
        private float _cooldownDelta;
        
        public override void Initialize()
        {
            _playerManager = PlayerManager.Instance;
            if (!_ui) _ui = FindObjectOfType<FarmUI>();
            _currentGeneration = _currentLevel == 1 ? InitialGeneration : ResourcesGeneration[_currentLevel - 2];
        }

        public override void Interact()
        {
            _ui.Open(FarmType, _generationResource, this);
        }

        protected override void OnLevelUp()
        {
            _currentGeneration = ResourcesGeneration[_currentLevel - 2];
        }

        public override void Boost()
        {
            if (_currentLevel == 1) _currentGeneration -= InitialGeneration * 0.25f;
            else _currentGeneration -= ResourcesGeneration[_currentLevel - 2] * 0.25f;
        }

        public override void EndBoost()
        {
            if (_currentLevel == 1) _currentGeneration += InitialGeneration * 0.25f;
            else _currentGeneration += ResourcesGeneration[_currentLevel - 2] * 0.25f;
        }

        public void Produce()
        {
            if(!_generationResource) return;
            
            _cooldownDelta -= Time.deltaTime;
            
            if (_cooldownDelta >= 0f) return;
            
            _playerManager.AddResource(_generationResource.GetKey(), 1);
            _cooldownDelta = _currentGeneration;
        }

        /// <summary>
        /// Gets the resource that the farm is generating
        /// </summary>
        /// <returns>The resource that the farm is generating</returns>
        public Resource GetResource() => _generationResource;

        /// <summary>
        /// Sets the resource to be generated
        /// </summary>
        /// <param name="resource">The resource that is going to generate this farm</param>
        public void Resource(Resource resource) => _generationResource = resource;

        /// <summary>
        /// Gets the generation based on the level of the building.
        /// </summary>
        /// <param name="level">Level of the building</param>
        /// <returns>The time (in seconds) between the generation of 1 resource</returns>
        public float GetGeneration() => _currentGeneration;

    }
}