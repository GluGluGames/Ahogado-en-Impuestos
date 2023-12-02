using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Containers
{
    public class AchievementContainer : MonoBehaviour
    {
        [SerializeField] private TMP_Text AchievementTitle;
        [SerializeField] private TMP_Text AchievementDescription;
        [SerializeField] private Image AchievementIcon;

        public void SetAchievementTitle(string title) => AchievementTitle.SetText(title);
        public void SetAchievementDescription(string desc) => AchievementDescription.SetText(desc);
        public void SetAchievementIcon(Sprite icon) => AchievementIcon.sprite = icon;

        public void SetAchievement(string title, string desc, Sprite icon)
        {
            SetAchievementTitle(title);
            SetAchievementDescription(desc);
            SetAchievementIcon(icon);
        }

    }
}
