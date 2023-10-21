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
        public List<EnemyBasic> enemies;
        private List<HexTile> tiles = new List<HexTile>();
        [SerializeField] private int _nEnemies = 0;
        [SerializeField] private EnemyBasic[] enemiesPrefab;

        // Start is called before the first frame update
        private void Start()
        {
            tiles = TileManager.instance.GetComponentsInChildren<HexTile>().ToList();

            for (int i = 0; i < _nEnemies; i++)
            {
                SpawnEnemy();
            }
        }

        private bool SpawnEnemy() // Should take some enum as input to define type of enemy
        {
            HexTile hex = TileManager.instance.GetRandomHex();
            bool spawned = false;

            foreach (EnemyBasic enemy in enemies)
            {
                if (enemy.currentTile == hex)
                {
                    spawned = SpawnEnemy();
                    break;
                }
            }
            if(enemiesPrefab != null && spawned == false)
            {
                EnemyBasic newEnem = Instantiate(enemiesPrefab[0]); // here is where you should use that enum...
                newEnem.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y+ 1f, hex.transform.position.z);
                newEnem.currentTile = hex;
                
                enemies.Add(newEnem);
            }
            

            return true;
        }
    }
}