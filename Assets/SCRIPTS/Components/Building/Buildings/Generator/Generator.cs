using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Buildings;
using UnityEngine;

namespace GGG.Components.Generator
{
    public class Generator : Building
    {
        private GeneratorUI _ui;
        
        public override void Interact(int level)
        {
            if(!_ui) _ui = FindObjectOfType<GeneratorUI>();

            _ui.Open();
        }
    }
}
