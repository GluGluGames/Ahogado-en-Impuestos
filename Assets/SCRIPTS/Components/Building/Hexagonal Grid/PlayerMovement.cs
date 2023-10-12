using UnityEngine;

namespace GGG.Components.Buildings
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Singleton

        public static PlayerMovement Instance;

        private void Awake()
        {
            Debug.Log("me genero");
            if (Instance != null) return;

            Instance = this;
        }

        #endregion Singleton

        public Vector3Int playerPos = Vector3Int.zero;
        public HexTile currentTile; 

        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}