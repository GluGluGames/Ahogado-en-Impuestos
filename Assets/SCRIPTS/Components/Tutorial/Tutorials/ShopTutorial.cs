using GGG.Components.Buildings.Shop;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class ShopTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            ShopUI.OnShopOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            ShopUI.OnShopOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
