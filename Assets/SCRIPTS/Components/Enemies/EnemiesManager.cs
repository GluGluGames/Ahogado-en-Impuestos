using DG.Tweening;
using GGG.Components.HexagonalGrid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GGG.Components.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        
        [SerializeField] private Enemy[] EnemiesPrefab;

        private readonly List<Enemy> _enemies = new();
        private int _nEnemies;

        private IEnumerator Start()
        {
            // This is made so the scene charges first and then the start method is called.
            yield return null;

            _nEnemies = EnemiesPrefab.Length;

            for (int i = 0; i < _nEnemies; i++)
            {
                SpawnEnemy();
            }
        }

        private bool SpawnEnemy()
        {
            HexTile hex = TileManager.Instance.GetRandomHex();
            bool spawned = false;

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.currentTile == hex)
                {
                    spawned = SpawnEnemy();
                    break;
                }
            }
            
            if (EnemiesPrefab != null && spawned == false)
            {
                int enemIndex = Random.Range(0, EnemiesPrefab.Length);
                Enemy newEnem = Instantiate(EnemiesPrefab[enemIndex], transform);
                newEnem.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
                newEnem.currentTile = hex;
                newEnem.isDirty = true;
                _enemies.Add(newEnem);
            }


            return true;
        }
    }
}