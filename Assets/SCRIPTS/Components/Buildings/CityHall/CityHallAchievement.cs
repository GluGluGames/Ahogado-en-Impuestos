using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallAchievement : MonoBehaviour
    {
        [SerializeField] private TMP_Text AchievementTitle;
        [SerializeField] private TMP_Text AchievementDescription;
        [SerializeField] private Image AchievementIcon;
        [SerializeField] private GameObject AchievementUnlock;

        public void SetAchievement(string title, string desc, Sprite icon, bool unlock)
        {
            AchievementTitle.SetText(title);
            AchievementDescription.SetText(desc);
            AchievementIcon.sprite = icon;
            AchievementUnlock.SetActive(!unlock);

            AchievementTitle.color = unlock ? new Color(0, 0.28f, 0.06f) : new Color(0.3f, 0.02f, 0f);
        }
    }
}
