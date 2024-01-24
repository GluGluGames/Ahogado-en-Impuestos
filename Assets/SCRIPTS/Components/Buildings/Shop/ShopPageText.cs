using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings.Shop
{
    [RequireComponent(typeof(TMP_Text))]
    public class ShopPageText : MonoBehaviour
    {
        private TMP_Text _pageText;
        private List<ShopPanelArrow> _arrows = new ();
        private Shop _shop;
        private int _level;

        private void OnEnable()
        {
            if (!_pageText) _pageText = GetComponent<TMP_Text>();
            if (_arrows.Count <= 0) _arrows = FindObjectsOfType<ShopPanelArrow>().ToList();
            
            if (!_shop) _shop = FindObjectOfType<Shop>();
            _level = _shop ? _shop.CurrentLevel() : 3;
            
            _arrows.ForEach(x => x.OnExchangesChange += SetText);
            
            SetText(1);
        }

        private void OnDisable()
        {
            _arrows.ForEach(x => x.OnExchangesChange -= SetText);
        }

        private void SetText(int page)
        {
            _pageText.SetText($"{page}/{_level}");
        }
    }
}
