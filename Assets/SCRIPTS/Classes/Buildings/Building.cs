using UnityEngine;
using GGG.Shared;

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
        [Tooltip("The price to build the building")]
        [SerializeField] private int PrimaryPrice;
        [SerializeField] private Resource PrimaryPriceType;
        [Tooltip("The price to build the building")]
        [SerializeField] private int SecondaryPrice;
        [SerializeField] private Resource SecondaryPriceType;
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
        /// <param name="parent">The parent where the building will spawn</param>
        /// <returns>The instance of the building</returns>
        public GameObject Spawn(Vector3 position, Transform parent)
        {
            return Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z), Quaternion.Euler(0, -45, 0), parent);
        }

        /// <summary>
        /// Gets the primary price of the building
        /// </summary>
        /// <returns>The primary price</returns>
        public int GetPrimaryPrice() { return PrimaryPrice; }
        
        /// <summary>
        /// Gets the secondary price of the building
        /// </summary>
        /// <returns>The secondary price</returns>
        public int GetSecondaryPrice() { return SecondaryPrice; }
        
        /// <summary>
        /// Gets the primary resource
        /// </summary>
        /// <returns>The primary resource</returns>
        public Resource GetPrimaryResource() { return PrimaryPriceType; }
        
        /// <summary>
        /// Gets the secondary resource
        /// </summary>
        /// <returns>The secondary resource</returns>
        public Resource GetSecondaryResource() { return SecondaryPriceType; }

        /// <summary>
        /// Determines if the building needs player interaction to work
        /// </summary>
        /// <returns>True if it needs player interaction. False otherwise</returns>
        public bool NeedInteraction() { return NeededInteraction; }

    }
}
