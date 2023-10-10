using GGG.Classes.Shop;
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

        private bool _open;

        private void Start()
        {
            Viewport.SetActive(false);
            transform.position = new Vector3(_INITIAL_POSITION + 960, 540);

            _player = PlayerManager.Instance;

            CloseButton.onClick.AddListener(CloseShop);

            // TODO - See if the game has already started and have generated exchanges
            UpdateTrades();
        }

        private void UpdateTrades()
        {
            for(int i = 0; i < Exchanges.Count; i++) {
                GiveItemImage[i].sprite = Exchanges[i].GetBasicResource().GetSprite();
                GiveAmountText[i].SetText(Exchanges[i].GetGivenAmount().ToString());

                ReceiveItemImage[i].sprite = Exchanges[i].GetAdvanceResource().GetSprite();
                ReceiveAmountText[i].SetText(Exchanges[i].GetReceiveAmount().ToString());
            }
        }

        public void IncreaseExchange(int i)
        {
            // TODO - Limit increase
            Exchanges[i].SetGivenAmount(Exchanges[i].GetGivenAmount() * _MAGNIFICATION_FACTOR);
            Exchanges[i].SetReceiveAmount(Exchanges[i].GetReceiveAmount() * _MAGNIFICATION_FACTOR);
            UpdateTrades();
        }

        public void DecreaseExchange(int i)
        {
            // TODO - Limit decrease
            Exchanges[i].SetGivenAmount(Exchanges[i].GetGivenAmount() / _MAGNIFICATION_FACTOR);
            Exchanges[i].SetReceiveAmount(Exchanges[i].GetReceiveAmount() / _MAGNIFICATION_FACTOR);
            UpdateTrades();
        }

        public void Exchange(int i)
        {
            if(_player.GetResourceCount(Exchanges[i].GetBasicResource().GetResource()) < Exchanges[i].GetGivenAmount()) {
                // TODO - Denegate exchange
                return;
            }

            _player.AddResource(Exchanges[i].GetBasicResource().GetResource(), -Exchanges[i].GetGivenAmount());
            _player.AddResource(Exchanges[i].GetAdvanceResource().GetResource(), Exchanges[i].GetReceiveAmount());
        }

        public void OpenShop()
        {
            if(_open) return;

            Viewport.SetActive(true);
            transform.DOMoveX(960, 2f, true).SetEase(Ease.InOutExpo);
            _open = true;
        }

        public void CloseShop()
        {
            if(!_open) return;

            transform.DOMoveX(_INITIAL_POSITION + 960, 2f, true).SetEase(Ease.InOutExpo).onComplete += () => { 
                Viewport.SetActive(false);
                _open = false;
            };
            
        }
    }
}
