using GGG.Components.Buildings;
using GGG.Components.Ticks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Enemies
{
    public class EnemyBasic : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _hp;
        [SerializeField] private bool _killable;
        [SerializeField] private bool _stoppable;

        private Rigidbody _rigidbody;

        private bool _moving;
        private bool _gotPath;

        public HexTile currentTile;
        public HexTile nextTile;
        public Vector3 targetPosition;
        public List<HexTile> currentPath = new List<HexTile>();
        public Vector3Int cubeCoordPos;

        void Start()
        {
            TickManager.OnTick += this.HandleMovement;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            TickManager.OnTick -= this.HandleMovement;
        }

        void Update()
        {
            if(!_gotPath)
            {
                GoToRandomTile();
            }
        }

        private void GoToRandomTile()
        {
            HexTile destination = TileManager.instance.GetRandomHex();
            targetPosition = destination.transform.position;
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null ) { _gotPath = true; }
            
        }

        private void MoveTo(Vector3 targetPos)
        {
            Quaternion angle = Quaternion.Euler(0, Vector3.Angle(targetPos, transform.forward), 0);
            _rigidbody.Move(targetPos, angle);

        }

        private void HandleMovement()
        {
            if (currentPath == null || currentPath.Count <= 1)
            {
                nextTile = null;

                if (currentPath != null && currentPath.Count > 0)
                {
                    currentTile = currentPath[0];
                    nextTile = currentTile;
                }

                _gotPath = false;

            }
            else
            {
                currentTile = currentPath[0];

                nextTile = currentPath[0];

                // if the next tile is not traversable, stop moving;
                /*if (PlayerPosition.NextTile.tileType != HexTileGenerationSettings.TileType.Standard)
                {
                    PlayerPosition.currentPath.Clear();
                    HandleMovement();
                    return;
                }*/

                targetPosition = nextTile.transform.position + new Vector3(0, 1f, 0);
                MoveTo(targetPosition);
                _gotPath = true;
                currentPath.RemoveAt(0);
                cubeCoordPos = nextTile.cubeCoordinate;
            }
        }
    }

}

