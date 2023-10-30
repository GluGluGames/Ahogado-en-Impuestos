using System;
using UnityEngine;
using GGG.Shared;

namespace GGG.Classes.Buildings
{
    [Serializable]
    public abstract class Building : ScriptableObject {
        [Header("Generic fields")]
        [Tooltip("Name of the building")] 
        [SerializeField] private string Name;
        [Tooltip("Description of the building")]
        [SerializeField] [TextArea] private string Description;
        [Tooltip("Icon of the building. Used in the UI")]
        [SerializeField] private Sprite Icon;
        [Tooltip("Initial prefab of the building")]
        [SerializeField] private GameObject Prefab;
        [Tooltip("Determines if the building can be upgrades")] 
        [SerializeField] private bool CanBeUpgraded;
        [Tooltip("Upgrade prefabs of the building. If the building don't have an upgrade, leave it empty")]
        [SerializeField] private GameObject[] UpgradePrefabs;
        [Space(10)] [Header("Building Fields")] 
        [Tooltip("The price to build the building")] 
        [SerializeField] private ResourceCost BuildCost;
        [Tooltip("Price to upgrade the building.")] 
        [SerializeField] private ResourceCost[] UpgradeCost;
        [Tooltip("Max level the build can be upgraded")] 
        [SerializeField] private int MaxLevel;
        [Tooltip("Determines if the player needs to click the building to interact with it")]
        [SerializeField] private bool NeededInteraction;
        [Tooltip("Determines the height of the building")]
        [SerializeField] private float SpawnHeight;
        [Tooltip("Determines the vision range of the building")] 
        [SerializeField] private int VisionRange;

        /// <summary>
        /// Interacts with the building
        /// </summary>
        public abstract void Interact(int level);

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
        /// Checks if the building can be upgraded
        /// </summary>
        /// <returns>True if it can be upgraded. False otherwise</returns>
        public bool CanUpgraded() => CanBeUpgraded;

        /// <summary>
        /// Spawns the building
        /// </summary>
        /// <param name="position">The position where the building will be spawned</param>
        /// <param name="parent">The parent where the building will spawn</param>
        /// <param name="level">Current level of the building</param>
        /// <param name="upgrade">If the building is being upgraded</param>
        /// <returns>The instance of the building</returns>
        public GameObject Spawn(Vector3 position, Transform parent, int level, bool upgrade)
        {
            if (!upgrade)
            {
                if(level == 1)
                    return Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z),
                    Quaternion.identity, parent);
                
                GameObject go = Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z), Quaternion.identity, parent);
                Destroy(go.transform.GetChild(0).gameObject);
                Instantiate(UpgradePrefabs[level - 2], new Vector3(position.x, SpawnHeight, position.z), Quaternion.identity, go.transform);
                
                return go;
            }

            Destroy(parent.GetChild(0).gameObject);
            Instantiate(UpgradePrefabs[level - 2], new Vector3(position.x, SpawnHeight, position.z), Quaternion.Euler(0, -45, 0), parent);
            return null;
        }

        /// <summary>
        /// Gets the total cost of the building.
        /// </summary>
        /// <returns>The cost of the building</returns>
        public ResourceCost GetBuildingCost() => BuildCost;

        /// <summary>
        /// Gets the total upgrade cost of the building
        /// </summary>
        /// <returns>The upgrade cost of the building</returns>
        public ResourceCost[] GetUpgradeCost() => UpgradeCost;

        /// <summary>
        /// Gets the cost of the building.
        /// </summary>
        /// <param name="index">0: Primary. 1: Secondary. 2: Tertiary. 3: Fourth</param>
        /// <returns>The cost of the building</returns>
        /// <exception cref="Exception">if the index is incorrect</exception>
        public int GetBuildingCost(int index)
        {
            if (index > 3)
                throw new Exception($"No building cost with {index} index");

            return BuildCost.GetCost(index);
        }

        /// <summary>
        /// Gets the upgrade cost of the building.
        /// </summary>
        /// <param name="level">The level of the building. Must be greater than 1 </param>
        /// <param name="costIndex">0: Primary. 1: Secondary. 2: Tertiary. 3: Fourth</param>
        /// <returns>The upgrade cost</returns>
        /// <exception cref="Exception">If the index is incorrect</exception>
        public int GetUpgradeCost(int level, int costIndex)
        {
            if (costIndex > 3)
                throw new Exception($"No building cost with {costIndex} index");

            return UpgradeCost[level - 1].GetCost(costIndex);
        }

        /// <summary>
        /// Gets the resource needed to build the building.
        /// </summary>
        /// <param name="index">0: Primary. 1: Secondary. 2: Tertiary. 3: Fourth</param>
        /// <returns>The resource needed to build</returns>
        /// <exception cref="Exception">If the index is incorrect</exception>
        public Resource GetBuildResource(int index)
        {
            if (index > 3)
                throw new Exception($"No building cost with {index} index");

            return BuildCost.GetResource(index);
        }

        /// <summary>
        /// Gets the resource needed to upgrade the building.
        /// </summary>
        /// <param name="level">The level of the upgrade</param>
        /// <param name="index">0: Primary. 1: Secondary. 2: Tertiary. 3: Fourth</param>
        /// <returns>The resource needed to upgrade</returns>
        /// <exception cref="Exception">If the index is incorrect</exception>
        public Resource GetUpgradeResource(int level, int index)
        {
            if (index > 3)
                throw new Exception($"No building cost with {index} index");

            return UpgradeCost[level - 1].GetResource(index);
        }

        /// <summary>
        /// Gets the max level of the building
        /// </summary>
        /// <returns>Max level of the building</returns>
        public int GetMaxLevel() => MaxLevel;

        /// <summary>
        /// Determines if the building needs player interaction to work
        /// </summary>
        /// <returns>True if it needs player interaction. False otherwise</returns>
        public bool NeedInteraction() { return NeededInteraction; }

        /// <summary>
        /// Gets the vision range of the building
        /// </summary>
        /// <returns>The vision range</returns>
        public int GetVisionRange() => VisionRange;

        

    }
}
