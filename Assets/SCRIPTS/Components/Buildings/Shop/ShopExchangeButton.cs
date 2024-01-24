using System;
using GGG.Components.Player;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Shop
{
    [RequireComponent(typeof(Button))]
    public class ShopExchangeButton : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;

        private PlayerManager _player;
        private ShopExchanges _exchanges;
        
        public Action OnExchange;

        private void OnEnable()
        {
            if (!_player) _player = PlayerManager.Instance;
            if (!_exchanges) _exchanges = GetComponentInParent<ShopExchanges>();
        }

        public void Exchange()
        {
            if (_player.GetResourceCount(_exchanges.Exchange(Index).GetGivenResource().GetKey()) < _exchanges.Exchange(Index).GetGivenAmount())
                return;
            
            _player.AddResource(_exchanges.Exchange(Index).GetGivenResource().GetKey(), 
                -_exchanges.Exchange(Index).GetGivenAmount());
            _player.AddResource(_exchanges.Exchange(Index).GetReceiveResource().GetKey(),
                _exchanges.Exchange(Index).GetReceiveAmount());
            
            OnExchange?.Invoke();
        }
    }
}
