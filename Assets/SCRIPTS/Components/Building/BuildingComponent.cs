using GGG.Classes.Buildings;
using System;
using UnityEngine;

namespace GGG.Components.Buildings {
    public class BuildingComponent : MonoBehaviour {
        [SerializeField] private Building Build;

        public Action<Action, BuildingComponent> OnBuildInteract;

        private void Update() {
            if (Build.NeedInteraction()) return;
            
            Build.Interact();
        }

        public void Interact() { OnBuildInteract?.Invoke(Build.Interact, this); }

        public bool NeedInteraction() { return Build.NeedInteraction(); }
    }
}
