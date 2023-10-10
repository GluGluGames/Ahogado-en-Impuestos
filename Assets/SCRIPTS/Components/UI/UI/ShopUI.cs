using GGG.Shared;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace GGG.Components.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private List<Image> GiveItemImage;
        [SerializeField] private List<Image> ReceiveItemImage;
        [SerializeField] private List<TMP_Text> GiveAmountText;
        [SerializeField] private List<TMP_Text> ReceiveAmountText;
        [SerializeField] private List<Button> MoreButtons;
        [SerializeField] private List<Button> LessButtons;

        private const int MAX_EXCHANGES = 3;

        private PlayerManager _player;

        private System.Random _random = new();
        private List<Dictionary<int, int>> _exchanges;
        private int _timeToExchange; 

        private void Start()
        {
            gameObject.SetActive(false);

            _player = PlayerManager.Instance;

            foreach (Button button in MoreButtons)
                button.onClick.AddListener(IncreaseExchange);
            foreach (Button button in LessButtons)
                button.onClick.AddListener(DecreaseExchange);

            // TODO - See if the game has already started and have generated exchanges
            GenerateInitialExchanges();

        }

        private void GenerateInitialExchanges()
        {
            BasicResources generatedBasicResource;
            AdvanceResources generatedAdvanceResource;
            Resource basicResource;

            int basicMax = System.Enum.GetNames(typeof(BasicResources)).Length;
            int advanceMax = System.Enum.GetNames(typeof(AdvanceResources)).Length;
            int next = 0;
            // TODO - Calculate items ratios

            for(int i = 0; i < MAX_EXCHANGES; i++)
            {
                next = _random.Next(0, basicMax - 1);
                generatedBasicResource = (BasicResources) next;
                basicResource = _player.GetResource(generatedBasicResource);

                // GiveItemImage[i].sprite = resource.GetSprite();
                // GiveAmountText[i].SetText();

                next = _random.Next(0, advanceMax - 1);
                generatedAdvanceResource = (AdvanceResources) next;
                basicResource = _player.GetResource(generatedAdvanceResource);

                // ReceiveItemImage[i].sprite = resource.GetSprite();
                // ReceiveAmountText[i].SetText();

            }
        }

        private void IncreaseExchange()
        {

        }

        private void DecreaseExchange()
        {

        }
    }
}
