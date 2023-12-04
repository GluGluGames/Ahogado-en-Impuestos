using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHall : BuildingComponent
    {
        private CityHallUi _ui;
        
        public override void Initialize()
        {
            if (!_ui) _ui = FindObjectOfType<CityHallUi>();
        }

        public override void Interact()
        {
            _ui.Open();
        }
    }
}
