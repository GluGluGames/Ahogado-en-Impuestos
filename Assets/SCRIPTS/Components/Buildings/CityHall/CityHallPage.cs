using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Achievements;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings.CityHall
{
    [RequireComponent(typeof(TMP_Text))]
    public class CityHallPage : MonoBehaviour
    {
        private List<CityHallArrow> _arrows = new ();
        private TMP_Text _text;

        private void OnEnable()
        {
            if (_arrows.Count <= 0) _arrows = FindObjectsOfType<CityHallArrow>().ToList();
            _arrows.ForEach(x => x.OnPageChange += ChangePage);
            if (!_text) _text = GetComponent<TMP_Text>();
            
            ChangePage(1);
        }

        private void OnDisable()
        {
            _arrows.ForEach(x => x.OnPageChange -= ChangePage);
        }

        private void ChangePage(int page)
        {
            _text.SetText($"{page}/{AchievementsManager.Instance.GetAchievements().Count / 3}");
        }
    }
}
