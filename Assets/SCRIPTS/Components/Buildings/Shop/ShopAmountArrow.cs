using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Shop
{
    [RequireComponent(typeof(Button))]
    public class ShopAmountArrow : MonoBehaviour
    {
        private enum Direction
        {
            Decrease,
            Increase
        }
        
        private const int _MAGNIFICATION_FACTOR = 2;

        [SerializeField] private Direction ArrowDirection;
        [SerializeField, Range(0, 2)] private int Index;
        
        private ShopExchanges _exchange;

        public Action OnExchangeChange;

        private void OnEnable()
        {
            if (!_exchange) _exchange = GetComponentInParent<ShopExchanges>();
        }

        public void OnArrowPress()
        {
            if(ArrowDirection == Direction.Increase) IncreaseExchange();
            else DecreaseExchange();
        }
        
        private void IncreaseExchange()
        {
            _exchange.Exchange(Index).SetGivenAmount(_exchange.Exchange(Index).GetGivenAmount() * _MAGNIFICATION_FACTOR);
            _exchange.Exchange(Index).SetReceiveAmount(_exchange.Exchange(Index).GetReceiveAmount() * _MAGNIFICATION_FACTOR);
            
            OnExchangeChange?.Invoke();
        }
        
        private void DecreaseExchange()
        {
            int givenAmount = _exchange.Exchange(Index).GetGivenAmount() / _MAGNIFICATION_FACTOR;
            int receiveAmount = _exchange.Exchange(Index).GetReceiveAmount() / _MAGNIFICATION_FACTOR;

            int initialGiven = _exchange.InitialGivenAmount(Index);
            int initialReceive = _exchange.InitialReceiveAmount(Index);
            
            _exchange.Exchange(Index).SetGivenAmount(givenAmount < initialGiven ? initialGiven : givenAmount);
            _exchange.Exchange(Index).SetReceiveAmount(receiveAmount < initialReceive ? initialReceive : receiveAmount);
            
            OnExchangeChange?.Invoke();
        }
    }
}
