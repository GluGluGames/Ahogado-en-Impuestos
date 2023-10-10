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
        [SerializeField] private List<Button> MoreButtons;
        [SerializeField] private List<Button> LessButtons;
        [SerializeField] private List<Button> ExchangeButtons;
        [SerializeField] private Button CloseButton;

        private const float _INITIAL_POSITION = -1600f;

        private PlayerManager _player;

        private bool _open;

        private void Start()
        {
            Viewport.SetActive(false);
            transform.position = new Vector3(_INITIAL_POSITION + 960, 540);

            _player = PlayerManager.Instance;

            foreach (Button button in MoreButtons)
                button.onClick.AddListener(IncreaseExchange);
            foreach (Button button in LessButtons)
                button.onClick.AddListener(DecreaseExchange);

            CloseButton.onClick.AddListener(CloseShop);

            // TODO - See if the game has already started and have generated exchanges
            InitializeTrades();
        }

        private void InitializeTrades()
        {
            for(int i = 0; i < Exchanges.Count; i++) {
                GiveItemImage[i].sprite = Exchanges[i].GetBasicResource().GetSprite();
                GiveAmountText[i].SetText(Exchanges[i].GetGivenAmount().ToString());

                ReceiveItemImage[i].sprite = Exchanges[i].GetAdvanceResource().GetSprite();
                ReceiveAmountText[i].SetText(Exchanges[i].GetReceiveAmount().ToString());
            }
        }

        private void IncreaseExchange()
        {

        }

        private void DecreaseExchange()
        {

        }

        private void Exchange(ShopExchange exchange)
        {
            if(_player.GetResourceCount(exchange.GetBasicResource().GetResource()) < exchange.GetGivenAmount()) {
                // TODO - Denegate exchange
                return;
            }

            _player.AddResource(exchange.GetBasicResource().GetResource(), -exchange.GetGivenAmount());
            _player.AddResource(exchange.GetAdvanceResource().GetResource(), exchange.GetReceiveAmount());
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
