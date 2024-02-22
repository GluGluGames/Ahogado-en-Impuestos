using System;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using UnityEngine;
using UnityEngine.UI;


namespace GGG.Components.UI.TileClean
{
    public class TileCleanButton : MonoBehaviour
    {
        private HexTile _tile;
        private Button _button;
        private bool _canClean;

        public Action OnClean;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(HexTile tile)
        {
            _tile = tile;
            
            CheckButtonState();
        }

        public void Deinitialize()
        {
            _tile = null;
        }

        private void CheckButtonState()
        {
            bool condition = PlayerManager.Instance.GetResourceCount("Seaweed") >= _tile.GetClearCost();

            _button.interactable = condition;
            _button.image.color = condition ? Color.white : new Color(0.81f, 0.84f, 0.81f, 0.9f);
            _canClean = condition;
        }

        public void OnTileClean()
        {
            if(!_canClean) return;
            
            _tile.SetTileType(TileType.Standard);
            _tile.DeselectTile();
            PlayerManager.Instance.AddResource("Seaweed", -_tile.GetClearCost());
            foreach(HexTile tile in TileManager.Instance.GetHexTiles()) 
                tile.SetClearCost(Mathf.RoundToInt(tile.GetClearCost() + 25));
            PlayerPrefs.SetInt("TilesClean", PlayerPrefs.GetInt("TilesClean", 0) + 1);
            PlayerPrefs.Save();
            
            OnClean?.Invoke();
        }
    }
}
