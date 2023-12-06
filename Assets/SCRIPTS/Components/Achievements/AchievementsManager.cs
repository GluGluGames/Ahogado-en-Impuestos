using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.Core
{
    public class AchievementsManager : MonoBehaviour
    {
        public static AchievementsManager Instance;

        private void Awake()
        {
            if (Instance) return;

            Instance = this;
        }
        
        [SerializeField] private List<Achievement> Achievements;
        [SerializeField] private TMP_Text AchievementTitle;
        [SerializeField] private Image AchievementIcon;
        [SerializeField] private LocalizedString AchievementString;

        [SerializeField] private int PopupTime = 5;
        
        private GameObject _achievementPopup;

        private void OnValidate()
        {
            Achievements = Resources.LoadAll<Achievement>("Achievements").ToList();
        }

        private void OnEnable()
        {
            if (Achievements.Count <= 0)
                Achievements = Resources.LoadAll<Achievement>("Achievements").ToList();
        }

        private void Start()
        {
            _achievementPopup = transform.GetChild(0).gameObject;
            _achievementPopup.transform.position = new Vector3(Screen.width * 0.05f, Screen.height * 1.25f);
            _achievementPopup.SetActive(false);
        }

        public Achievement Achievement(int idx) => Achievements[idx];

        public List<Achievement> GetAchievements() => Achievements;

        public IEnumerator UnlockAchievement(string key)
        {
            Achievement achievement = Achievements.Find(x => x.GetKey() == key);
            if (!achievement)
                throw new Exception("Not achievement found");
            if (achievement.IsUnlocked()) yield break;
            
            achievement.Unlock();
            
            AchievementTitle.SetText($"{AchievementString.GetLocalizedString()} {achievement.GetName()}");
            AchievementIcon.sprite = achievement.GetSprite();
            
            _achievementPopup.SetActive(true);
            _achievementPopup.transform.DOMoveY(Screen.height - 20, 2f).SetEase(Ease.InSine);
            yield return new WaitForSeconds(PopupTime);
            _achievementPopup.transform.DOMoveY(Screen.height * 1.25f, 2f).SetEase(Ease.OutSine).onComplete += () =>
            {
                _achievementPopup.SetActive(false);
            };
        }
    }
}
