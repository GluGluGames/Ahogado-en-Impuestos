using UnityEngine;
using UnityEngine.Serialization;

namespace GGG.Classes.Buildings
{
    public abstract class Building : ScriptableObject {
        [Header("Generic fields")]
        [Tooltip("Name of the building")] 
        [SerializeField] private string Name;
        [Tooltip("Description of the building")]
        [SerializeField] [TextArea] private string Description;
        [Tooltip("Icon of the building. Used in the UI")]
        [SerializeField] private Sprite Icon;
        [Tooltip("Prefab of the building")]
        [SerializeField] private GameObject Prefab;
        [Space(10)]
        [Header("Building Fields")]
        [Tooltip("Determines if the player needs to click the building to interact with it")]
        [SerializeField] private bool NeededInteraction;
        [Tooltip("Determines the height of the building")]
        [SerializeField] private float SpawnHeight;

        public abstract void Interact();

        public string GetName() { return Name; }
        public string GetDescription() { return Description; }
        public Sprite GetIcon() { return Icon; }
        public GameObject Spawn(Vector3 position) { return Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z), Quaternion.Euler(0, -45, 0)); }

        public bool NeedInteraction() { return NeededInteraction; }

    }
}
