using GGG.Components.Museum;
using UnityEngine;

namespace GGG.Classes.Buildings
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
