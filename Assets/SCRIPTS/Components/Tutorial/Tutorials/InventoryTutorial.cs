using GGG.Components.UI.Inventory;

namespace GGG.Components.Tutorial.Tutorials
{
    public class InventoryTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            InventoryUI.OnInventoryOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            InventoryUI.OnInventoryOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
