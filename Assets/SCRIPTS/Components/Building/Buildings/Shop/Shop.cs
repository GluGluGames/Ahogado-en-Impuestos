using GGG.Classes.Buildings;

using UnityEngine;

namespace GGG.Components.Shop
{
    [CreateAssetMenu(fileName = "Shop", menuName = "Game/Buildings/Shop")]
    public class Shop : Building
    {
        private ShopUI _shop;

        public override void Interact(int level)
        {
            _shop = FindObjectOfType<ShopUI>();

            _shop.OpenShop();
        }
    }
}
