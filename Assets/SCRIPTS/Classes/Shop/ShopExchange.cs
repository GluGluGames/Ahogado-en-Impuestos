using GGG.Shared;

using UnityEngine;
using System;

namespace GGG.Classes.Shop
{
    [Serializable]
    public class ShopExchange
    {
        [SerializeField] private BasicResource GivenResource;
        [SerializeField] private int GivenAmount;
        [SerializeField] private AdvanceResource ReceiveResource;
        [SerializeField] private int ReceiveAmount;

        public BasicResource GetBasicResource() { return GivenResource; }
        public AdvanceResource GetAdvanceResource() { return ReceiveResource; }
        public int GetGivenAmount() { return GivenAmount; }
        public int GetReceiveAmount() { return ReceiveAmount; }
    }
}
