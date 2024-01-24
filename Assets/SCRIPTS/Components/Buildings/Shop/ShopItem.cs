using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;
        [Header("Images")]
        [SerializeField] private Image GivenImage;
        [SerializeField] private Image ReceiveImage;
        [Header("Texts")]
        [SerializeField] private TMP_Text GivenText;
        [SerializeField] private TMP_Text ReceiveText;
        
        private ShopExchanges _exchanges;
        private List<ShopAmountArrow> _amountArrows = new ();

        private void OnEnable()
        {
            if (!_exchanges) _exchanges = GetComponentInParent<ShopExchanges>();
            _exchanges.OnExchangesGenerated += UpdateTrades;

            if (_amountArrows.Count <= 0) _amountArrows = GetComponentsInChildren<ShopAmountArrow>().ToList();
            _amountArrows.ForEach(x => x.OnExchangeChange += UpdateTrades);
        }

        private void OnDisable()
        {
            _amountArrows.ForEach(x => x.OnExchangeChange -= UpdateTrades);
        }

        public void UpdateTrades()
        {
            GivenImage.sprite = _exchanges.Exchange(Index).GetGivenResource().GetSprite();
            GivenText.SetText($"{_exchanges.Exchange(Index).GetGivenAmount().ToString()} " +
                              $"{_exchanges.Exchange(Index).GetGivenResource().GetName()}");

            ReceiveImage.sprite = _exchanges.Exchange(Index).GetReceiveResource().GetSprite();
            ReceiveText.SetText($"{_exchanges.Exchange(Index).GetReceiveAmount()} " +
                                $"{_exchanges.Exchange(Index).GetReceiveResource().GetName()}");
        }
    }
}
