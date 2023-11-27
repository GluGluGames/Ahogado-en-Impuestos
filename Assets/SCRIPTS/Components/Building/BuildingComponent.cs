using GGG.Classes.Buildings;
using GGG.Shared;

using System;
using UnityEngine;

namespace GGG.Components.Buildings {
    public abstract class BuildingComponent : MonoBehaviour {
        [Tooltip("Type of building")]
        [SerializeField] protected Building BuildingData;
        
        protected int _currentLevel = 1;
        private ResourceCost _currentCost;
        protected bool _boosted;
        private int _id;
        
        public Action<BuildingComponent> OnBuildSelect;

        public abstract void Initialize();

        /// <summary>
        /// Select the building
        /// </summary>
        public void Select()
        {
            OnBuildSelect?.Invoke(this);
        }

        public abstract void Interact();

        protected virtual void OnLevelUp()
        {
            throw new Exception($"OnLevelUp not implemented on {name}");
        }

        public virtual void Boost()
        {
            throw new Exception($"Boost not implemented on {name}");
        }

        public virtual void EndBoost()
        {
            throw new Exception($"Boost not implemented on {name}");
        }

        public virtual void OnBuildDestroy() { }

        /// <summary>
        /// Gets the id of the building
        /// </summary>
        /// <returns>The id of the building</returns>
        public int Id() => _id;

        /// <summary>
        /// Sets the id of the building
        /// </summary>
        /// <param name="id">The id of the building</param>
        public void SetId(int id) => _id = id;

        public ResourceCost CurrentCost() => _currentCost;

        public void SetCurrentCost(ResourceCost cost) => _currentCost = cost;

        /// <summary>
        /// Gets the current level of the building
        /// </summary>
        /// <returns>The current level of the building</returns>
        public int CurrentLevel() => _currentLevel;

        /// <summary>
        /// Sets the level of the building
        /// </summary>
        /// <param name="level">New level of the build</param>
        /// <exception cref="Exception">If the level is higher or lowest than the limits</exception>
        public void SetLevel(int level)
        {
            if (level < 1 || level > BuildingData.GetMaxLevel())
                throw new Exception("Incorrect level");
            
            _currentLevel = level;
        }

        /// <summary>
        /// Adds a level to the building
        /// </summary>
        /// <returns>True if the level could be added. False otherwise</returns>
        public void AddLevel()
        {
            if (_currentLevel + 1 > BuildingData.GetMaxLevel())
                throw new Exception("Max level reached");
            
            _currentLevel++;
            OnLevelUp();
        }

        /// <summary>
        /// Check if the building is being boosted
        /// </summary>
        /// <returns>True if the building is boost. False otherwise</returns>
        public bool IsBoost() => _boosted;

        /// <summary>
        /// Gets the building information
        /// </summary>
        /// <returns>The building information</returns>
        public Building BuildData() => BuildingData;

        /// <summary>
        /// Gets the current position of the building
        /// </summary>
        /// <returns>The current position of the building</returns>
        public Vector3 Position() => transform.position;

        /// <summary>
        /// Gets the vision range of the building
        /// </summary>
        /// <returns>The vision range</returns>
        public int VisionRange() => BuildingData.GetVisionRange();
    }
}
