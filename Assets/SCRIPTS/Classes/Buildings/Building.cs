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

        /// <summary>
        /// Interacts with the building
        /// </summary>
        public abstract void Interact();

        /// <summary>
        /// Gets the name of the building
        /// </summary>
        /// <returns>The name of the building</returns>
        public string GetName() { return Name; }

        /// <summary>
        /// Gets the description of the building
        /// </summary>
        /// <returns>The description of the building</returns>
        public string GetDescription() { return Description; }

        /// <summary>
        /// Gets the sprite of the building
        /// </summary>
        /// <returns>The sprite of the building</returns>
        public Sprite GetIcon() { return Icon; }

        /// <summary>
        /// Spawns the building
        /// </summary>
        /// <param name="position">The position where the building will be spawned</param>
        /// <returns>The instance of the building</returns>
        public GameObject Spawn(Vector3 position) { return Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z), Quaternion.Euler(0, -45, 0)); }

        /// <summary>
        /// Determines if the building needs player interaction to work
        /// </summary>
        /// <returns>True if it needs player interaction. False otherwise</returns>
        public bool NeedInteraction() { return NeededInteraction; }

    }
}
