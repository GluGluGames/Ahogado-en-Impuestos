using GGG.Components.Buildings;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Input;
using GGG.Shared;

using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class TileCleanUI : MonoBehaviour
    {
        [SerializeField] private Button CleanButton;
        [SerializeField] private Button CloseButton;
        [SerializeField] private GameObject Container;

        private PlayerManager _player;
        private InputManager _input;
        private Resource _cleanResource;
        private Transform _transform;
        private GameObject _viewport;
        private HexTile _selectedTile;

        private bool _open;

        private TextMeshProUGUI _costAmountText;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _cleanResource = _player.GetMainResource();
            
            CleanButton.onClick.AddListener(CleanTile);
            CloseButton.onClick.AddListener(Close);
            CloseButton.gameObject.SetActive(false);

            _transform = transform;
            _transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 360);

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            Container.GetComponentsInChildren<Image>()[1].sprite = _cleanResource.GetSprite();
            _costAmountText = Container.GetComponentInChildren<TextMeshProUGUI>();

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
            }
        }

        private void Update() {
            if (!_open || !_input.Escape()) return;

            Close();
        }

        private void CleanTile()
        {
            if (_player.GetResourceCount(_cleanResource.GetKey()) < _selectedTile.GetClearCost())
            {
                // TODO - Can't clear tile warning
                return;
            }
            
            _selectedTile.SetTileType(TileType.Standard);
            _player.AddResource(_cleanResource.GetKey(), -_selectedTile.GetClearCost());
            Close();
        }

        private void Open(HexTile tile)
        {
            if (_open || tile.GetTileType() is TileType.Standard or TileType.Build)
                return;

            _viewport.SetActive(true);
            _selectedTile = tile;
            _costAmountText.SetText(_selectedTile.GetClearCost().ToString());
            _open = true;
            CloseButton.gameObject.SetActive(true);
            GameManager.Instance.OnUIOpen();

            _transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f), 0.1f).SetEase(Ease.InCubic);
        }

        public void Close()
        {
            if (!_open) return;

            _transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 360), 0.1f).SetEase(Ease.InCubic).onComplete += () => {
                _viewport.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };

            _selectedTile.DeselectTile();
            _selectedTile = null;
            GameManager.Instance.OnUIClose();
            _open = false;
        }
    }
}
