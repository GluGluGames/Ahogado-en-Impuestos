using System;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.TileClean
{
    public class TileCleanResource : MonoBehaviour
    {
        private TMP_Text _costText;
        private Image _costResource;

        private void Awake()
        {
            _costText = GetComponentInChildren<TMP_Text>();
            _costResource = GetComponentInChildren<Image>();
        }

        public void Initialize(HexTile tile)
        {
            _costText.SetText(tile.GetClearCost().ToString());
            _costResource.sprite = PlayerManager.Instance.GetResource("Seaweed").GetSprite();
        }
    }
}
