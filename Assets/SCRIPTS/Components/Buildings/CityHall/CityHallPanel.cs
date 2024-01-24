using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Achievements;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallPanel : MonoBehaviour
    {
        private AchievementsManager _achievementsManager;
        private List<CityHallAchievement> _achievements = new ();
        private List<CityHallArrow> _arrows = new();

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            _achievementsManager = AchievementsManager.Instance;
            
            if (_achievements.Count <= 0) _achievements = GetComponentsInChildren<CityHallAchievement>().ToList();
            if (_arrows.Count <= 0) _arrows = FindObjectsOfType<CityHallArrow>().ToList();
            _arrows.ForEach(x => x.OnPageChange += ChangeAchievements);
            
            SetAchievements(0);
        }

        private void OnDisable()
        {
            _arrows.ForEach(x => x.OnPageChange -= ChangeAchievements);
        }

        private void ChangeAchievements(int page)
        {
            int index = (page - 1) * _achievements.Count;
            SetAchievements(index);
        }

        private void SetAchievements(int idx)
        {
            List<Achievement> achievements = _achievementsManager.GetAchievements();
            foreach (CityHallAchievement achievement in _achievements)
            {
                achievement.SetAchievement(achievements[idx].GetName(),
                    achievements[idx].GetDescription(),
                    achievements[idx].GetSprite(),
                    achievements[idx].IsUnlocked());

                idx++;
            }
        }
    }
}
