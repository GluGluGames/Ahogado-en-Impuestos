using GGG.Classes.Buildings;
using System;
using UnityEngine;

namespace GGG.Components.Buildings {
    public class BuildingComponent : MonoBehaviour {
        [Tooltip("Type of building")]
        [SerializeField] private Building Build;

        public Action<Action, BuildingComponent> OnBuildInteract;

        private void Update() {
            if (Build.NeedInteraction()) return;
            
            Build.Interact();
        }

        /// <summary>
        /// Interact with the building
        /// </summary>
        public void Interact() { OnBuildInteract?.Invoke(Build.Interact, this); }

        /// <summary>
        /// The building needs interaction to work
        /// </summary>
        /// <returns>True if it needs. False otherwise</returns>
        public bool NeedInteraction() { return Build.NeedInteraction(); }
        
        /// <summary>
        /// Gets the building cost
        /// </summary>
        /// <returns>The building cost</returns>
        public int GetBuildCost() { return Build.GetPrimaryPrice(); }

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
