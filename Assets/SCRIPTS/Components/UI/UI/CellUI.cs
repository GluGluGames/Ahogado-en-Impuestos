using System;
using DG.Tweening;
using GGG.Components.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI {
    public class CellUI : MonoBehaviour {
        private Cell _cell;
        
        private bool _open;

        private void Start() {
            _cell = GetComponentInParent<Cell>();

            _cell.OnCellClick += Open;
            
            gameObject.SetActive(false);
        }

        private void Open(BuildingComponent building) {
            if (_open || !building) return;

            gameObject.SetActive(true);
        }

        private void Close() {
            if (!_open) return;
            
            gameObject.SetActive(false);
        }
    }
}
