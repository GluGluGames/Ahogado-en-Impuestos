using GGG.Classes.Buildings;
using GGG.Shared;

using System;
using UnityEngine;

namespace GGG.Components.Buildings {
    public abstract class BuildingComponent : MonoBehaviour {
        [Tooltip("Type of building")]
        [SerializeField] protected Building BuildingData;
        
        public Action<BuildingComponent> OnBuildSelect;
        protected int CurrentLevel = 1;
        protected ResourceCost CurrentPrice;

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

        /// <summary>
        /// Gets the current level of the building
        /// </summary>
        /// <returns>The current level of the building</returns>
        public int GetCurrentLevel() => CurrentLevel;

        /// <summary>
        /// Sets the level of the building
        /// </summary>
        /// <param name="level">New level of the build</param>
        /// <exception cref="Exception">If the level is higher or lowest than the limits</exception>
        public void SetLevel(int level)
        {
            if (level < 1 || level > BuildingData.GetMaxLevel())
                throw new Exception("Incorrect level");
            
            CurrentLevel = level;
        }

        /// <summary>
        /// Adds a level to the building
        /// </summary>
        /// <returns>True if the level could be added. False otherwise</returns>
        public void AddLevel()
        {
            if (CurrentLevel + 1 > BuildingData.GetMaxLevel())
                throw new Exception("Max level reached");
            
            CurrentLevel++;
            OnLevelUp();
        }

        /// <summary>
        /// Gets the building information
        /// </summary>
        /// <returns>The building information</returns>
        public Building GetBuild() => BuildingData;

        /// <summary>
        /// Gets the current position of the building
        /// </summary>
        /// <returns>The current position of the building</returns>
        public Vector3 GetPosition() => transform.position;

        /// <summary>
        /// Gets the vision range of the building
        /// </summary>
        /// <returns>The vision range</returns>
        public int GetVisionRange() => BuildingData.GetVisionRange();
    }
}
