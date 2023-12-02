using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Shared;
using TMPro;
using UnityEngine;
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

        private const int _POPUP_TIME = 5;
        
        private GameObject _achievementPopup;
        private Vector3 _popupInitialPosition;

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
            _popupInitialPosition = _achievementPopup.transform.position;
        }

        public Achievement Achievement(int idx) => Achievements[idx];

        public IEnumerator UnlockAchievement(string key)
        {
            Achievement achievement = Achievements.Find(x => x.GetKey() == key);
            if (!achievement)
                throw new Exception("Not achievement found");
            
            achievement.Unlock();
            
            AchievementTitle.SetText(achievement.GetName());
            AchievementIcon.sprite = achievement.GetSprite();
            
            _achievementPopup.transform.DOMoveY(Screen.height - 20, 2f).SetEase(Ease.InBounce);
            yield return new WaitForSeconds(_POPUP_TIME);
            _achievementPopup.transform.DOMoveY(Screen.height * 1.25f, 2f).SetEase(Ease.OutBounce);
        }
    }
}
