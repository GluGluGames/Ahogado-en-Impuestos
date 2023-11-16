using GGG.Classes.Buildings;
using UnityEngine;

namespace GGG.Components.Museum
{
    [CreateAssetMenu(fileName = "Museum", menuName = "Game/Buildings/Museum")]
    public class Museum : Building
    {
        private MuseumUI _museum;
        
        public override void Interact(int level)
        {
            _museum = FindObjectOfType<MuseumUI>();

            _museum.Open();
        }
    }
}
