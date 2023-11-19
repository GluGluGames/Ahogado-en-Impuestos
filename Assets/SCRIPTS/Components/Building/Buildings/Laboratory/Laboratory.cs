using GGG.Classes.Buildings;
using UnityEngine;

namespace GGG.Components.Laboratory
{
    [CreateAssetMenu(fileName = "Laboratory", menuName = "Game/Buildings/Laboratory")]
    public class Laboratory : Building
    {
        private LaboratoryUI _ui;
        
        public override void Interact(int level)
        {
            _ui = FindObjectOfType<LaboratoryUI>();
            
            _ui.Open();
        }

        public override void Boost(int level) { }
        
        public override void EndBoost(int level) { }
    }
}
