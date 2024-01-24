using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Shop
{
    [RequireComponent(typeof(Button))]
    public class ShopPanelArrow : MonoBehaviour
    {
        private enum ArrowDirection
        {
            Up,
            Down
        }

        [SerializeField] private ArrowDirection Direction;

        private Shop _shop;
        private List<ShopItem> _shopItems = new ();
        private List<ShopPanelArrow> _arrows = new ();

        private int _direction;
        private int _page;
        private int _level;
        
        public Action<int> OnExchangesChange;

        private void OnEnable()
        {
            if (!_shop) _shop = FindObjectOfType<Shop>();
            _level =  _shop ? _shop.CurrentLevel() : 3;

            if (_shopItems.Count <= 0) _shopItems = FindObjectsOfType<ShopItem>(true).ToList();

            _direction = Direction == ArrowDirection.Up ? -1 : (int) ArrowDirection.Down;
            if (_arrows.Count <= 0) _arrows = FindObjectsOfType<ShopPanelArrow>().ToList();

            ShopUI.OnShopClose += Restore;
        }

        private void OnDisable()
        {
            ShopUI.OnShopClose -= Restore;
        }

        private void UpdatePage(int page) => _page = page;

        private void Restore()
        {
            _page = 1;
            
            OnExchangesChange?.Invoke(_page);
            
            _shopItems.ForEach(x => x.UpdateTrades());
            _arrows.ForEach(x => x.UpdatePage(_page));
        }

        public void ChangeExchanges()
        {
            if ((_page + _direction) * 3 > _level * 3 || _page + _direction <= 0) return;
            _page += _direction;
            
            OnExchangesChange?.Invoke(_page);
            
            _shopItems.ForEach(x => x.UpdateTrades());
            _arrows.ForEach(x => x.UpdatePage(_page));
        }
    }
}
