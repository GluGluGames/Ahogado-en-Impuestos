using System;
using System.Collections;
using GGG.Classes.Shop;
using GGG.Components.Core;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Essentials")]
        [SerializeField] private List<ShopExchange> Exchanges;
        [SerializeField] private int ExchangeTime;
        [SerializeField] private TMP_Text PageText;
        [SerializeField] private TMP_Text TimeText;
        [Space(5)]
        [Header("Given Fields")]
        [SerializeField] private List<Image> GiveItemImage;
        [SerializeField] private List<TMP_Text> GiveAmountText;
        [Space(5)]
        [Header("Receive Fields")]
        [SerializeField] private List<Image> ReceiveItemImage;
        [SerializeField] private List<TMP_Text> ReceiveAmountText;
        [Space(5)] [Header("Buttons Fields")] 
        [SerializeField] private Button UpArrow;
        [SerializeField] private Button DownArrow;
        [SerializeField] private Button CloseButton;
        
        private const int _MAGNIFICATION_FACTOR = 2;

        private PlayerManager _player;
        private GameManager _gameManager;
        private GameObject _viewport;
        private readonly Dictionary<int, List<int>> _initialGivenAmounts = new();
        private readonly Dictionary<int, List<int>> _initialReceiveAmounts = new();
        private readonly Dictionary<int, List<ShopExchange>> _levelExchanges = new();

        private bool _open;
        private int _currentLevel;
        private int _currentExchanges = 1;
        private float _delta;

        public static Action OnShopOpen;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);

            _delta = ExchangeTime;

            UpArrow.onClick.AddListener(() => ChangeExchanges(-1));
            DownArrow.onClick.AddListener(() => ChangeExchanges(1));
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Update()
        {
            if (!_open) return;
            
            int minutes = Mathf.FloorToInt(_delta / 60);
            int seconds = Mathf.FloorToInt(_delta % 60);
            TimeText.SetText($"{minutes:00}:{seconds:00}");
        }

        private void GenerateExchanges()
        {
            List<ShopExchange> remainingExchanges = new (Exchanges);
            List<ShopExchange> aux = new();

            for (int i = 0; i < _currentLevel; i++)
            {
                bool contains = _levelExchanges.ContainsKey(i + 1);

                if (!contains) _initialGivenAmounts.Add(i + 1, new List<int>());
                else _initialGivenAmounts[i + 1] = new List<int>();

                if (!contains) _initialReceiveAmounts.Add(i + 1, new List<int>());
                else _initialReceiveAmounts[i + 1] = new List<int>();
                
                for (int j = 0; j < 3; j++)
                {
                    int idx = Random.Range(0, remainingExchanges.Count);
                    aux.Add(remainingExchanges[idx]);
                    _initialGivenAmounts[i + 1].Add(remainingExchanges[idx].GetGivenAmount());
                    _initialReceiveAmounts[i + 1].Add(remainingExchanges[idx].GetReceiveAmount());
                    
                    remainingExchanges.RemoveAt(idx);
                }

                if (!contains) _levelExchanges.Add(i + 1, new List<ShopExchange>(aux));
                else _levelExchanges[i + 1] = new List<ShopExchange>(aux);
                aux.Clear();
            }
        }

        private void ChangeExchanges(int direction)
        {
            if ((_currentExchanges + direction) * 3 > _currentLevel * 3 || 
                _currentExchanges + direction <= 0) return;

            int idx = _currentExchanges + direction;
            
            for (int i = 0; i < GiveItemImage.Count; i++)
            {
                GiveItemImage[i].sprite = _levelExchanges[idx][i].GetGivenResource().GetSprite();
                GiveAmountText[i].SetText($"{_levelExchanges[idx][i].GetGivenAmount().ToString()} {_levelExchanges[idx][i].GetGivenResource().GetName()}");

                ReceiveItemImage[i].sprite = _levelExchanges[idx][i].GetReceiveResource().GetSprite();
                ReceiveAmountText[i].SetText($"{_levelExchanges[idx][i].GetReceiveAmount().ToString()} {_levelExchanges[idx][i].GetReceiveResource().GetName()}");
            }

            _currentExchanges += direction;
            PageText.SetText($"{_currentExchanges}/{_currentLevel}");
        }

        private void UpdateTrades()
        {
            int idx = _currentExchanges;
            
            for(int i = 0; i < GiveItemImage.Count; i++) {
                
                GiveItemImage[i].sprite = _levelExchanges[idx][i].GetGivenResource().GetSprite();
                GiveAmountText[i].SetText($"{_levelExchanges[idx][i].GetGivenAmount().ToString()} " +
                                          $"{_levelExchanges[idx][i].GetGivenResource().GetName()}");

                ReceiveItemImage[i].sprite = _levelExchanges[idx][i].GetReceiveResource().GetSprite();
                ReceiveAmountText[i].SetText($"{_levelExchanges[idx][i].GetReceiveAmount().ToString()} " +
                                             $"{_levelExchanges[idx][i].GetReceiveResource().GetName()}");
            }
        }

        public void IncreaseExchange(int i)
        {
            int idx = _currentExchanges;
            
            _levelExchanges[idx][i].SetGivenAmount(_levelExchanges[idx][i].GetGivenAmount() * _MAGNIFICATION_FACTOR);
            _levelExchanges[idx][i].SetReceiveAmount(_levelExchanges[idx][i].GetReceiveAmount() * _MAGNIFICATION_FACTOR);
            UpdateTrades();
        }

        public void DecreaseExchange(int i)
        {
            int idx = _currentExchanges;
            
            int givenDecreaseAmount = _levelExchanges[idx][i].GetGivenAmount() / _MAGNIFICATION_FACTOR;
            int receiveDecreaseAmount = _levelExchanges[idx][i].GetReceiveAmount() / _MAGNIFICATION_FACTOR;

            _levelExchanges[idx][i].SetGivenAmount(givenDecreaseAmount < _initialGivenAmounts[idx][i] 
                ? _initialGivenAmounts[idx][i] : givenDecreaseAmount);
            _levelExchanges[idx][i].SetReceiveAmount(receiveDecreaseAmount <= _initialReceiveAmounts[idx][i] 
                ? _initialReceiveAmounts[idx][i] : receiveDecreaseAmount);
            UpdateTrades();
        }

        public void Exchange(int i)
        {
            int idx = _currentExchanges;
            
            if(_player.GetResourceCount(_levelExchanges[idx][i].GetGivenResource().GetKey()) < _levelExchanges[idx][i].GetGivenAmount()) return;
            
            _player.AddResource(_levelExchanges[idx][i].GetGivenResource().GetKey(), -_levelExchanges[idx][i].GetGivenAmount());
            _player.AddResource(_levelExchanges[idx][i].GetReceiveResource().GetKey(), _levelExchanges[idx][i].GetReceiveAmount());
        }

        private IEnumerator ChangeExchanges()
        {
            while (_delta > 0)
            {
                _delta -= Time.deltaTime;
                yield return null;
            }

            _delta = ExchangeTime;
            GenerateExchanges();
            if(_open) UpdateTrades();
            StartCoroutine(ChangeExchanges());
        }

        public void OpenShop(int level)
        {
            if(_open) return;

            _viewport.SetActive(true);

            _currentLevel = level;
            if (_levelExchanges.Count < level) GenerateExchanges();
            UpdateTrades();
            PageText.SetText($"{_currentExchanges}/{_currentLevel}");
            if(_delta >= ExchangeTime) StartCoroutine(ChangeExchanges());
            OnShopOpen?.Invoke();
            
            _gameManager.OnUIOpen();
            _open = true;
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f, true).SetEase(Ease.OutCubic).onComplete += () => { 
                _viewport.SetActive(false);
                _open = false;
            };

            _currentExchanges = 1;
            _gameManager.OnUIClose();
        }
    }
}
