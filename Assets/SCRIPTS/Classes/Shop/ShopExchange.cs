using GGG.Shared;

using UnityEngine;
using System;

namespace GGG.Classes.Shop
{
    [Serializable]
    public class ShopExchange
    {
        [Tooltip("Resource the player will give")]
        [SerializeField] private Resource GivenResource;
        [Tooltip("Amount of the resource the player will give")]
        [SerializeField] private int GivenAmount;
        [Tooltip("Resource the player will receive")]
        [SerializeField] private Resource ReceiveResource;
        [Tooltip("Amount of the resource the player will receive")]
        [SerializeField] private int ReceiveAmount;

        /// <summary>
        /// Gets the resource the player will give
        /// </summary>
        /// <returns>The resource the player will give</returns>
        public Resource GetGivenResource() => GivenResource;

        /// <summary>
        /// Gets the resource the player will receive
        /// </summary>
        /// <returns>The resource the player will receive</returns>
        public Resource GetReceiveResource() => ReceiveResource;

        /// <summary>
        /// Gets the amount of the resource the player will give
        /// </summary>
        /// <returns>Amount of the resource the player will give</returns>
        public int GetGivenAmount() { return GivenAmount; }

        /// <summary>
        /// Gets the amount of the resource the player will receive
        /// </summary>
        /// <returns>Amount of the resource the player will receive</returns>
        public int GetReceiveAmount() { return ReceiveAmount; }

        /// <summary>
        /// Determines the amount of resource the player will give
        /// </summary>
        /// <param name="amount">Amount of the resource the player will give</param>
        public void SetGivenAmount(int amount) { GivenAmount = amount; }

        /// <summary>
        /// Determines the amount of resource the player will receive
        /// </summary>
        /// <param name="amount">Amount of the resource the player will receive</param>
        public void SetReceiveAmount(int amount) { ReceiveAmount = amount; }
    }
}
