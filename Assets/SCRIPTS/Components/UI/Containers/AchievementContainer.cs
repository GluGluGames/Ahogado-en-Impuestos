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
        [SerializeField] private GameObject AchievementUnlock;

        public void SetAchievementTitle(string title) => AchievementTitle.SetText(title);
        public void SetAchievementDescription(string desc) => AchievementDescription.SetText(desc);
        public void SetAchievementIcon(Sprite icon) => AchievementIcon.sprite = icon;
        public void SetAchievementUnlockState(bool unlock) => AchievementUnlock.SetActive(unlock);

        public void SetAchievement(string title, string desc, Sprite icon, bool unlock)
        {
            SetAchievementTitle(title);
            SetAchievementDescription(desc);
            SetAchievementIcon(icon);
            SetAchievementUnlockState(!unlock);

            AchievementTitle.color = unlock ? new Color(0, 0.28f, 0.06f) : new Color(0.3f, 0.02f, 0f);
        }

    }
}
