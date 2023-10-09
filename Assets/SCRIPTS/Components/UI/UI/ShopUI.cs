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

        private List<Dictionary<int, int>> _exchanges;
        private int _timeToExchange;

        private void Start()
        {
            foreach (Button button in MoreButtons)
                button.onClick.AddListener(IncreaseExchange);

            // TODO - See if the game has already started and have generated exchanges
            GenerateInitialExchanges();
        }

        private void GenerateInitialExchanges()
        {

        }

        private void IncreaseExchange()
        {

        }
    }
}
