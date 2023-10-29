using GGG.Components.Shop;

using UnityEngine;

namespace GGG.Classes.Buildings
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
