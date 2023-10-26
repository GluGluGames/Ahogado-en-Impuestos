using GGG.Components.Buildings;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Generic fields")]
        [Tooltip("Name of the enemy")]
        [SerializeField] private string Name;

        [Tooltip("Description of the enemy")]
        [SerializeField][TextArea] private string Description;

        [Tooltip("Icon of the enemy. Used in the UI")]
        [SerializeField] private Sprite Icon;

        [Tooltip("Prefab of the enemy")]
        [SerializeField] protected GameObject Prefab;

        [Space(10)]
        [Header("Enemy Fields")]
        [Tooltip("Determines the height of the enemy")]
        [SerializeField] protected float SpawnHeight;

        [SerializeField] protected int hp;
        [SerializeField] protected bool isStoppable;
        [SerializeField] protected bool _alwaysVisible;

        protected Rigidbody Rb;
        public HexTile currentTile;
        public Vector3Int cubeCoordPos;
        public bool isDirty = false;

        ///<summary>
        /// Gets the name of the enemy
        /// </summary>
        /// <returns>The name of the enemy</returns>
        public string GetName()
        { return Name; }

        /// <summary>
        /// Gets the description of the enemy
        /// </summary>
        /// <returns>The description of the enemy</returns>
        public string GetDescription()
        { return Description; }

        /// <summary>
        /// Gets the sprite of the enemy
        /// </summary>
        /// <returns>The sprite of the enemy</returns>
        public Sprite GetIcon()
        { return Icon; }

        /// <summary>
        /// Spawns the enemy
        /// </summary>
        /// <param name="position">The position where the enemy will be spawned</param>
        /// <returns>The instance of the enemy</returns>
        public GameObject Spawn(Vector3 position)
        { return Instantiate(Prefab, new Vector3(position.x, SpawnHeight, position.z), Quaternion.Euler(0, 0, 0)); }

    }
}