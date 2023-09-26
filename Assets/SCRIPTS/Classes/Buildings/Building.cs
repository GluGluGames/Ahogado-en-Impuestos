using UnityEngine;
using UnityEngine.Serialization;

namespace GGG.Classes.Buildings
{
    public abstract class Building : ScriptableObject {
        [Tooltip("Name of the building")] 
        [SerializeField] private string Name;
        [Tooltip("Description of the building")]
        [SerializeField] [TextArea] private string Description;
        [Tooltip("Icon of the building. Used in the UI")]
        [SerializeField] private Sprite Icon;
        [Tooltip("Prefab of the building")]
        [SerializeField] private GameObject Prefab;
        [Tooltip("Determines if the player needs to click the building to interact with it")]
        [SerializeField] private bool NeededInteraction;

        public abstract void Interact();

        public string GetName() { return Name; }
        public string GetDescription() { return Description; }
        public Sprite GetIcon() { return Icon; }
        public GameObject Spawn(Vector3 position) { return Instantiate(Prefab, position, Quaternion.identity); }

        public bool NeedInteraction() { return NeededInteraction; }

    }
}
