using GGG.Components.Buildings.CityHall;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class CityHallTutorial : Tutorial
    {
        protected override void TutorialCondition()
        {
            CityHallUi.OnCityHallOpen += StartTutorialNoEnum;
        }

        protected override void FinishTutorial()
        {
            CityHallUi.OnCityHallOpen -= StartTutorialNoEnum;
            base.FinishTutorial();
        }
    }
}
