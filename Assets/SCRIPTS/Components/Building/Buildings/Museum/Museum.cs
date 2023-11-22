using GGG.Components.Buildings;
using UnityEngine;

namespace GGG.Components.Buildings.Museum
{
    public class Museum : BuildingComponent
    {
        private MuseumUI _museum;

        public override void Initialize()
        {
            if(!_museum) _museum = FindObjectOfType<MuseumUI>();
        }

        public override void Interact()
        {
            _museum.Open();
        }
    }
}
