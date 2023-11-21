using System;
using GGG.Classes.Shop;
using GGG.Components.Core;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

namespace GGG.Components.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Essentials")]
        [SerializeField] private List<ShopExchange> Exchanges;
        [Space(5)]
        [Header("Given Fields")]
        [SerializeField] private List<Image> GiveItemImage;
        [SerializeField] private List<TMP_Text> GiveAmountText;
        [Space(5)]
        [Header("Receive Fields")]
        [SerializeField] private List<Image> ReceiveItemImage;
        [SerializeField] private List<TMP_Text> ReceiveAmountText;
        [Space(5)]
        [Header("Buttons Fields")]
        [SerializeField] private Button CloseButton;
        
        private const int _MAGNIFICATION_FACTOR = 2;

        private PlayerManager _player;
        private GameManager _gameManager;
        private GameObject _viewport;
        private readonly List<int> _initialGivenAmounts = new();
        private readonly List<int> _initialReceiveAmounts = new();

        private bool _open;

        public static Action OnShopOpen;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            
            foreach(ShopExchange exchange in Exchanges) {
                _initialGivenAmounts.Add(exchange.GetGivenAmount());
                _initialReceiveAmounts.Add(exchange.GetReceiveAmount());
            }

            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void UpdateTrades()
        {
            for(int i = 0; i < Exchanges.Count; i++) {
                GiveItemImage[i].sprite = Exchanges[i].GetGivenResource().GetSprite();
                GiveAmountText[i].SetText($"{Exchanges[i].GetGivenAmount().ToString()} {Exchanges[i].GetGivenResource().GetName()}");

                ReceiveItemImage[i].sprite = Exchanges[i].GetReceiveResource().GetSprite();
                ReceiveAmountText[i].SetText($"{Exchanges[i].GetReceiveAmount().ToString()} {Exchanges[i].GetReceiveResource().GetName()}");
            }
        }

        public void IncreaseExchange(int i)
        {
            Exchanges[i].SetGivenAmount(Exchanges[i].GetGivenAmount() * _MAGNIFICATION_FACTOR);
            Exchanges[i].SetReceiveAmount(Exchanges[i].GetReceiveAmount() * _MAGNIFICATION_FACTOR);
            UpdateTrades();
        }

        public void DecreaseExchange(int i)
        {
            int givenDecreaseAmount = Exchanges[i].GetGivenAmount() / _MAGNIFICATION_FACTOR;
            int receiveDecreaseAmount = Exchanges[i].GetReceiveAmount() / _MAGNIFICATION_FACTOR;

            Exchanges[i].SetGivenAmount(givenDecreaseAmount < _initialGivenAmounts[i] ? _initialGivenAmounts[i] : givenDecreaseAmount);
            Exchanges[i].SetReceiveAmount(receiveDecreaseAmount < _initialReceiveAmounts[i] ? _initialReceiveAmounts[i] : receiveDecreaseAmount);
            UpdateTrades();
        }

        public void Exchange(int i)
        {
            if(_player.GetResourceCount(Exchanges[i].GetGivenResource().GetKey()) < Exchanges[i].GetGivenAmount()) return;
            
            _player.AddResource(Exchanges[i].GetGivenResource().GetKey(), -Exchanges[i].GetGivenAmount());
            _player.AddResource(Exchanges[i].GetReceiveResource().GetKey(), Exchanges[i].GetReceiveAmount());
        }

        public void OpenShop()
        {
            if(_open) return;

            _viewport.SetActive(true);
            
            UpdateTrades();
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
            
            _gameManager.OnUIClose();
        }
    }
}
