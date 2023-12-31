using DG.Tweening;
using GGG.Components.HexagonalGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GGG.Components.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {

        [SerializeField] private int _maxEnemies;

        [SerializeField] private Enemy[] EnemiesPrefab;

        private readonly List<Enemy> _enemies = new();
        private List<HexTile> _tiles;
        private int _nEnemies;

        private void Start()
        {

            StartCoroutine(DelayedStart());

        }

        IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.4f);

            _nEnemies = EnemiesPrefab.Length;
            _tiles = FindObjectsOfType<HexTile>().ToList();

            for (int i = 0; i < _maxEnemies; i++)
            {
                SpawnEnemy();
            }

        }

        private bool SpawnEnemy()
        {
            HexTile hex = _tiles[Random.Range(0, _tiles.Count)];
            bool spawned = false;

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.currentTile != hex && hex != PlayerPosition.CurrentTile && hex.GetTileType() != TileType.Mountain && hex.selectable == true) continue;
                
                spawned = SpawnEnemy();
                break;
            }

            if (EnemiesPrefab == null || spawned != false) return true;
            
            int enemIndex = Random.Range(0, EnemiesPrefab.Length);
            Enemy newEnem = Instantiate(EnemiesPrefab[enemIndex], transform);
            newEnem.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
            newEnem.currentTile = hex;
            newEnem.isDirty = true;
            _enemies.Add(newEnem);
            
            return true;
        }
    }
}