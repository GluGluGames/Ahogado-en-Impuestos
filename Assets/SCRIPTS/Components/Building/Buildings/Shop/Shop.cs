namespace GGG.Components.Buildings.Shop
{
    public class Shop : BuildingComponent
    {
        private ShopUI _shop;

        public override void Initialize()
        {
            if(!_shop) _shop = FindObjectOfType<ShopUI>();
        }

        public override void Interact()
        {
            _shop.OpenShop(CurrentLevel());
        }

        public override void OnBuildDestroy()
        {
            _shop.StopAllCoroutines();
        }
    }
}
