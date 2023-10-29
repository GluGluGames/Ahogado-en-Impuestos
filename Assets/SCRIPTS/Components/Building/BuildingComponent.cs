using GGG.Classes.Buildings;
using System;
using UnityEngine;

namespace GGG.Components.Buildings {
    public class BuildingComponent : MonoBehaviour {
        [Tooltip("Type of building")]
        [SerializeField] private Building Build;

        public Action<Action, BuildingComponent> OnBuildInteract;
        private HexTile _currentTile;
        private int _currentLevel = 1;

        private void Update() {
            if (Build.NeedInteraction()) return;
            
            Build.Interact(_currentLevel);
        }

        /// <summary>
        /// Interact with the building
        /// </summary>
        public void Interact() { OnBuildInteract?.Invoke(() => Build.Interact(_currentLevel), this); }

        /// <summary>
        /// Gets the current level of the building
        /// </summary>
        /// <returns>The current level of the building</returns>
        public int GetCurrentLevel() => _currentLevel;

        /// <summary>
        /// Sets the level of the building
        /// </summary>
        /// <param name="level">New level of the build</param>
        /// <exception cref="Exception">If the level is higher or lowest than the limits</exception>
        public void SetLevel(int level)
        {
            if (level < 1 || level > Build.GetMaxLevel())
                throw new Exception("Incorrect level");
            
            _currentLevel = level;
        }

        /// <summary>
        /// Adds a level to the building
        /// </summary>
        /// <returns>True if the level could be added. False otherwise</returns>
        public bool AddLevel()
        {
            if (_currentLevel + 1 > Build.GetMaxLevel())
                return false;

            _currentLevel++;
            return true;
        }

        /// <summary>
        /// Gets the building information
        /// </summary>
        /// <returns>The building information</returns>
        public Building GetBuild() => Build;

        /// <summary>
        /// The building needs interaction to work
        /// </summary>
        /// <returns>True if it needs. False otherwise</returns>
        public bool NeedInteraction() { return Build.NeedInteraction(); }

        /// <summary>
        /// Gets the tile where the building is builded
        /// </summary>
        /// <returns>The hextile</returns>
        public HexTile GetCurrentTile() => _currentTile;

        public void SetTile(HexTile tile) => _currentTile = tile;

        /// <summary>
        /// Gets the current position of the building
        /// </summary>
        /// <returns>The current position of the building</returns>
        public Vector3 GetPosition() => transform.position;

        /// <summary>
        /// Gets the vision range of the building
        /// </summary>
        /// <returns>The vision range</returns>
        public int GetVisionRange() => Build.GetVisionRange();
    }
}
