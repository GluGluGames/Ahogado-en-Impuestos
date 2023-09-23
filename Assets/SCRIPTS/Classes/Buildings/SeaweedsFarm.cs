using GGG.Components.Player;
using UnityEngine;
using System;

namespace GGG.Classes.Buildings
{
    [CreateAssetMenu(menuName = "Game/SeaweedFarm", fileName = "SeaweedFarm")]
    public class SeaweedsFarm : Building {
        [SerializeField] private int InitialGeneration;
        [SerializeField] private int InitialCooldown;

        private float _cooldownDelta;
        
        
        public override void Interact() {
            if (_cooldownDelta >= 0) {
                _cooldownDelta -= Time.deltaTime;
                return;
            }
            
            PlayerManager.Instance.AddSeaweeds(InitialGeneration);
            _cooldownDelta = InitialCooldown;
        }
    }
}
