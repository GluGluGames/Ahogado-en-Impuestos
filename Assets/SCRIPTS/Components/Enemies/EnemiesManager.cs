using DG.Tweening;
using GGG.Components.Buildings;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {

        public static EnemiesManager instance;
        public List<Enemy> enemies;
        private List<HexTile> tiles = new List<HexTile>();
        [SerializeField] private int _nEnemies = 0;
        [SerializeField] private Enemy[] enemiesPrefab;

        // Start is called before the first frame update
        private void Start()
        {
            tiles = TileManager.instance.GetComponentsInChildren<HexTile>().ToList();

            for (int i = 0; i < _nEnemies; i++)
            {
                SpawnEnemy();
            }
        }

        private bool SpawnEnemy()
        {
            HexTile hex = TileManager.instance.GetRandomHex();
            bool spawned = false;

            foreach (Enemy enemy in enemies)
            {
                if (enemy.currentTile == hex)
                {
                    spawned = SpawnEnemy();
                    break;
                }
            }
            if (enemiesPrefab != null && spawned == false)
            {
                Enemy newEnem = Instantiate(enemiesPrefab[0], transform);
                newEnem.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
                newEnem.currentTile = hex;
                newEnem.isDirty = true;
                enemies.Add(newEnem);
            }


            return true;
        }
    }
}