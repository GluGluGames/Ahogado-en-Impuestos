using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings.Shop
{
    public class ShopTimer : MonoBehaviour
    {
        [SerializeField] private int ExchangeTime;
        [SerializeField] private TMP_Text TimerText;
        private ShopUI _ui;

        private float _delta;
        private bool _updateTimer;
        private bool _timerRunning;

        public Action OnTimerEnd;

        private void Start()
        {
            _delta = ExchangeTime;
            if (!_ui) _ui = GetComponentInParent<ShopUI>();
            
            ShopUI.OnShopOpen += EnableTimer;
            ShopUI.OnShopClose += DisableTimer;
        }

        private void Update()
        {
            if (!_updateTimer) return;
            
            int minutes = Mathf.FloorToInt(_delta / 60);
            int seconds = Mathf.FloorToInt(_delta % 60);
            TimerText.SetText($"{minutes:00}:{seconds:00}");
        }

        private void OnDisable()
        {
            ShopUI.OnShopOpen -= EnableTimer;
            ShopUI.OnShopClose -= DisableTimer;
        }

        private void DisableTimer()
        {
            _updateTimer = false;
        }

        private void EnableTimer()
        {
            _updateTimer = true;
            if (!_timerRunning) StartCoroutine(ChangeExchanges());
        }

        private IEnumerator ChangeExchanges()
        {
            _timerRunning = true;
            
            while (_delta > 0)
            {
                _delta -= Time.deltaTime;
                yield return null;
            }

            _delta = ExchangeTime;
            OnTimerEnd?.Invoke();
            StartCoroutine(ChangeExchanges());
        }
    }
}
