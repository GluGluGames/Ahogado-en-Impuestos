using GGG.Classes.Buildings;
using UnityEngine;

namespace GGG.Components.Buildings {
    public class BuildingComponent : MonoBehaviour {
        [SerializeField] private Building Build;

        private void Update() {
            if (Build.NeedInteraction()) return;
            
            Build.Interact();
        }
    }
}
