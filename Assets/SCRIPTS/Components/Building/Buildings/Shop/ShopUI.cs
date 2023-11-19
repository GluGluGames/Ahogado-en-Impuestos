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
        [Header("Essencials")]
        [SerializeField] private GameObject Viewport;
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

        private const float _INITIAL_POSITION = -1600f;
        private const int _MAGNIFICATION_FACTOR = 2;

        private PlayerManager _player;
        private List<int> _initialGivenAmounts = new();
        private List<int> _initialReceiveAmounts = new();

        private bool _open;

        public static Action OnShopOpen;

        private void Start()
        {
            Viewport.SetActive(false);
            transform.position = new Vector3(Screen.width * 0.5f + _INITIAL_POSITION, Screen.height * 0.5f);

            _player = PlayerManager.Instance;

            int i = 0;
            foreach(ShopExchange exchange in Exchanges) {
                _initialGivenAmounts.Add(exchange.GetGivenAmount());
                _initialReceiveAmounts.Add(exchange.GetReceiveAmount());

                GiveItemImage[i].sprite = exchange.GetGivenResource().GetSprite();
                ReceiveItemImage[i].sprite = exchange.GetReceiveResource().GetSprite();

                i++;
            }

            CloseButton.onClick.AddListener(CloseShop);

            // TODO - See if the game has already started and have generated exchanges
            UpdateTrades();
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
            if(_player.GetResourceCount(Exchanges[i].GetGivenResource().GetName()) < Exchanges[i].GetGivenAmount()) {
                // TODO - Denegate exchange
                return;
            }

            _player.AddResource(Exchanges[i].GetGivenResource().GetName(), -Exchanges[i].GetGivenAmount());
            _player.AddResource(Exchanges[i].GetReceiveResource().GetName(), Exchanges[i].GetReceiveAmount());
        }

        public void OpenShop()
        {
            if(_open) return;

            Viewport.SetActive(true);
            transform.DOMoveX(Screen.width * 0.5f, 2f, true).SetEase(Ease.InOutExpo);
            GameManager.Instance.OnUIOpen();
            OnShopOpen?.Invoke();
            _open = true;
        }

        public void CloseShop()
        {
            if(!_open) return;

            transform.DOMoveX(Screen.width * 0.5f + _INITIAL_POSITION, 2f, true).SetEase(Ease.InOutExpo).onComplete += () => { 
                Viewport.SetActive(false);
                _open = false;
            };
            
            GameManager.Instance.OnUIClose();
        }
    }
}
