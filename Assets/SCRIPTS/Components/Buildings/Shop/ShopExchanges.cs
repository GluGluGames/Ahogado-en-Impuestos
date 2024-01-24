using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Shop;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings.Shop
{
    public class ShopExchanges : MonoBehaviour
    {
        [SerializeField] private List<ShopExchange> Exchanges;

        private Shop _shop;
        private List<ShopPanelArrow> _panelArrow = new();
        private ShopTimer _timer;
        
        private readonly Dictionary<int, List<int>> _initialGivenAmounts = new();
        private readonly Dictionary<int, List<int>> _initialReceiveAmounts = new();
        private readonly Dictionary<int, List<ShopExchange>> _levelExchanges = new();

        private int _level;
        private int _page = 1;

        public Action OnExchangesGenerated;

        private void OnEnable()
        {
            if (!_timer) _timer = GetComponentInChildren<ShopTimer>();
            _timer.OnTimerEnd += GenerateExchanges;
            
            if (_panelArrow.Count <= 0) _panelArrow = GetComponentsInChildren<ShopPanelArrow>().ToList();
            _panelArrow.ForEach(x => x.OnExchangesChange += UpdatePage);
            
            ShopUI.OnShopOpen += Initialize;
            ShopUI.OnShopClose += Restore;
        }

        private void OnDisable()
        {
            ShopUI.OnShopOpen -= Initialize;
            ShopUI.OnShopClose -= Restore;
            
            _panelArrow.ForEach(x => x.OnExchangesChange -= UpdatePage);
            _timer.OnTimerEnd -= GenerateExchanges;
        }

        private void Initialize()
        {
            if (!_shop) _shop = FindObjectOfType<Shop>();
            _level = _shop ? _shop.CurrentLevel() : 3;
            
            if (_levelExchanges.Count < _level) GenerateExchanges();
        }

        private void Restore()
        {
            _initialGivenAmounts.Clear();
            _initialReceiveAmounts.Clear();

            _page = 1;
        }

        public ShopExchange Exchange(int index) => _levelExchanges[_page][index];
        public int InitialGivenAmount(int index) => _initialGivenAmounts[_page][index];
        public int InitialReceiveAmount(int index) => _initialReceiveAmounts[_page][index];

        private void UpdatePage(int page) => _page = page;

        private void GenerateExchanges()
        {
            List<ShopExchange> remaining = new (Exchanges);
            List<ShopExchange> aux = new ();

            for (int i = 0; i < _level; i++)
            {
                bool contains = _levelExchanges.ContainsKey(i + 1);

                if (!contains) _initialGivenAmounts.Add(i + 1, new List<int>());
                else _initialGivenAmounts[i + 1] = new List<int>();

                if (!contains) _initialReceiveAmounts.Add(i + 1, new List<int>());
                else _initialGivenAmounts[i + 1] = new List<int>();

                for (int j = 0; j < 3; j++)
                {
                    int idx = Random.Range(0, remaining.Count);
                    aux.Add(remaining[idx]);
                    _initialGivenAmounts[i + 1].Add(remaining[idx].GetGivenAmount());
                    _initialReceiveAmounts[i + 1].Add(remaining[idx].GetReceiveAmount());
                    remaining.RemoveAt(idx);
                }

                if (contains) _levelExchanges[i + 1] = new List<ShopExchange>(aux);
                else _levelExchanges.Add(i + 1, new List<ShopExchange>(aux));
                aux.Clear();
            }
            
            OnExchangesGenerated?.Invoke();
        }
    }
}
