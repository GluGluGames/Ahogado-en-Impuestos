using GGG.Components.Player;
using GGG.Shared;

using UnityEngine;
using System;

namespace GGG.Classes.Buildings
{
    [CreateAssetMenu(menuName = "Game/SeaweedFarm", fileName = "Farm")]
    public class Farm : Building {
        [SerializeField] private Resource Resource;
        [SerializeField] private int InitialGeneration;
        [SerializeField] private int InitialCooldown;

        private float _cooldownDelta;
        
        
        public override void Interact() {
            if (_cooldownDelta >= 0) {
                _cooldownDelta -= Time.deltaTime;
                return;
            }
            
            PlayerManager.Instance.AddResource(Resource.GetResource().ToString(), InitialGeneration);
            _cooldownDelta = InitialCooldown;
        }
    }
}