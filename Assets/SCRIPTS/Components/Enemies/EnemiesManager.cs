using DG.Tweening;
using GGG.Components.Buildings;
using System.Collections;
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

        private IEnumerator Start()
        {
            // This is made so the scene charges first and then the start method is called.
            yield return new WaitWhile(() => !TileManager.instance.aux);

            // tiles = TileManager.instance.GetComponentsInChildren<HexTile>().ToList();

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
                int enemIndex = Random.Range(0, enemiesPrefab.Length);
                Enemy newEnem = Instantiate(enemiesPrefab[enemIndex], transform);
                // newEnem.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
                newEnem.currentTile = hex;
                newEnem.isDirty = true;
                enemies.Add(newEnem);
            }


            return true;
        }
    }
}