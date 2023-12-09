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
                    tilesBorder.OffsetY();
                }

                if (GUILayout.Button("ResetScaleY"))
                {
                    tilesBorder.ResetY();
                }
            }
        }


            
        

        [SerializeField] private List<Transform> tilesFathers;
        private List<Transform> tiles = new List<Transform>();
        [SerializeField] private float MaxScaleY;

        private void OffsetY()
        {
            float offsetY;
            GetAllTiles();

            foreach (Transform t in tiles)
            {
                offsetY = Random.Range(0, MaxScaleY);
                t.localScale = new Vector3(t.localScale.x, t.localScale.y + offsetY, t.localScale.z);
            }
        }

        private void ResetY()
        {
            GetAllTiles();

            foreach (Transform t in tiles)
            {
                t.localScale = new Vector3(t.localScale.x, 1, t.localScale.z);
            }
        }

        private void GetAllTiles()
        {
            foreach (Transform father in tilesFathers)
            {
                int hexTileCount = father.childCount;
                for(int i = 0; i < hexTileCount; i++)
                {
                    tiles.Add(GetChildrenTile(father.GetChild(i)));
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