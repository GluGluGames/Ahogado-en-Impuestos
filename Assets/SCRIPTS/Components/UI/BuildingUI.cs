using UnityEngine;
using DG.Tweening;
using GGG.Components.Buildings;
using UnityEngine.UI;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private bool _open;
    
        private void Start() {
            CloseButton.onClick.AddListener(Close);
            
            Cell[] cells = FindObjectsOfType<Cell>();

            foreach (Cell cell in cells) {
                cell.OnCellClick += Open;
            }

            _open = false;
            transform.position = new Vector3(0, -400f, 0);
        }

        private void Open() {
            if (_open) return;
            
            transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = true;
        }

        private void Close() {
            if (!_open) return;
            
            transform.DOMove(new Vector3(0f, -400, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = false;
        }
    }
}
