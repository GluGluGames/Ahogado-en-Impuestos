using GGG.Components.Player;
using GGG.Shared;

using UnityEngine;
using System;

namespace GGG.Classes.Buildings
{
    [CreateAssetMenu(menuName = "Game/Buildings/Farm", fileName = "Farm")]
    public class Farm : Building {
        [SerializeField] private BasicResource Resource;
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
