using GGG.Components;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.HexagonalGrid
{
    public class HexTilesBorderOffset : MonoBehaviour
    {
        #if UNITY_EDITOR

        [CustomEditor(typeof(HexTilesBorderOffset))]
        public class customInspectorGUI : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                HexTilesBorderOffset tilesBorder = (HexTilesBorderOffset)target;
                if (GUILayout.Button("OffsetY"))
                {
                    tilesBorder.Tiles = new List<Transform>();
                    tilesBorder.OffsetY();
                }

                if (GUILayout.Button("ResetScaleY"))
                {
                    tilesBorder.Tiles = new List<Transform>();
                    tilesBorder.ResetY();
                }

                if (GUILayout.Button("StandarizeAllTiles (NO RETURN!!!!!!)"))
                {
                    tilesBorder.StandarizeTile();
                }
            }
        }


            
        

        [SerializeField] private List<Transform> tilesFathers;
        private List<Transform> Tiles;
        [SerializeField] private float MaxScaleY;
        [SerializeField] private float NormalScaleY;
        [SerializeField] private GameObject standardTile;

        private void OffsetY()
        {
            float offsetY;
            GetAllTiles();

            foreach (Transform t in Tiles)
            {
                offsetY = Random.Range(0, MaxScaleY);
                t.localScale = new Vector3(t.localScale.x, NormalScaleY + offsetY, t.localScale.z);
            }
        }

        private void ResetY()
        {
            GetAllTiles();

            foreach (Transform t in Tiles)
            {
                t.localScale = new Vector3(t.localScale.x, NormalScaleY, t.localScale.z);
            }
        }

        private void StandarizeTile()
        {
            if(standardTile == null)
            {
                Debug.LogError("NO PREFAB FOUND FOR STANDARD TILE");
                return;
            }

            foreach (Transform father in tilesFathers)
            {
                int hexTileCount = father.childCount;
                for(int i = 0; i < hexTileCount; i++)
                {
                    Transform tile = father.GetChild(i).GetChild(0);
                    Vector3 position = tile.position;
                    if (!tile.name.Contains("HexagonNonconstructible4") || tile.childCount > 0)
                    {
                        DestroyImmediate(tile.gameObject);
                        GameObject go = Instantiate(standardTile, position, Quaternion.identity, father.GetChild(i));
                        go.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    }
                }

            }   
        }

        private void GetAllTiles()
        {
            foreach (Transform father in tilesFathers)
            {
                int hexTileCount = father.childCount;
                for(int i = 0; i < hexTileCount; i++)
                {
                    Tiles.Add(GetChildrenTile(father.GetChild(i)));
                }

            }    
        }

        private Transform GetChildrenTile(Transform go)
        {
            if(go.childCount == 0)
            { return go; }
            else
            {
                return GetChildrenTile(go.GetChild(0));
            }

        }
    #endif
    }
}