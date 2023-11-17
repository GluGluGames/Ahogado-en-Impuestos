using GGG.Components.Player;
using GGG.Shared;

using UnityEngine;
using System;

namespace GGG.Classes.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/Farm", fileName = "Farm")]
    public class Farm : Building {
        [Tooltip("Resource to be generated by the farm")]
        [SerializeField] private Resource Resource;
        [Tooltip("Time of resource generation without upgrades")]
        [SerializeField] private float InitialGeneration;
        [Tooltip("Time of resource generation with the upgrades")] 
        [SerializeField] private float[] ResourcesGeneration;

        private float _cooldownDelta;
        
        public override void Interact(int level) {
            _cooldownDelta -= Time.deltaTime;
            
            if (_cooldownDelta >= 0f) return;
            
            PlayerManager.Instance.AddResource(Resource.GetKey(), 1);
            _cooldownDelta = level == 1 ? InitialGeneration : ResourcesGeneration[level - 2];
        }

        public override void Boost(int level)
        {
            if(!CanBeBoost()) return;

            if (level == 1) InitialGeneration -= InitialGeneration * 0.25f;
            else ResourcesGeneration[level - 2] -= ResourcesGeneration[level - 2] * 0.25f;
        }

        public override void EndBoost(int level)
        {
            if (level == 1) InitialGeneration += InitialGeneration * 0.25f;
            else ResourcesGeneration[level - 2] += ResourcesGeneration[level - 2] * 0.25f;
        }

        /// <summary>
        /// Gets the generated resource
        /// </summary>
        /// <returns>The generated resource</returns>
        public Resource GetResource() => Resource;

        /// <summary>
        /// Gets the generation based on the level of the building.
        /// </summary>
        /// <param name="level">Level of the building</param>
        /// <returns>The time (in seconds) between the generation of 1 resource</returns>
        public float GetGeneration(int level) => level == 1 ? InitialGeneration : ResourcesGeneration[level - 2];
    }
}
