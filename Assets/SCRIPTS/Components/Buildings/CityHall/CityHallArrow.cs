using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Achievements;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    [RequireComponent(typeof(Button))]
    public class CityHallArrow : MonoBehaviour
    {
        private enum Direction
        {
            Up,
            Down
        }

        [SerializeField] private Direction ArrowDirection;

        private List<CityHallArrow> _arrows = new ();

        private int _page = 1;
        private int _direction;

        public Action<int> OnPageChange;

        private void OnEnable()
        {
            if (_arrows.Count <= 0) _arrows = FindObjectsOfType<CityHallArrow>().ToList();
            
            _direction = ArrowDirection == Direction.Up ? -1 : (int) ArrowDirection;

            CityHallUi.OnCityHallClose += Restore;
        }

        private void OnDisable()
        {
            CityHallUi.OnCityHallClose -= Restore;
        }

        private void Restore()
        {
            _page = 1;

            OnPageChange?.Invoke(_page);
            UpdateArrows();
        }

        public void ChangeAchievements()
        {
            if ((_page + _direction) * 3 > AchievementsManager.Instance.GetAchievements().Count || _page + _direction <= 0) return;
            _page += _direction;
            
            OnPageChange?.Invoke(_page);
            UpdateArrows();
        }
        
        private void UpdateArrows() => _arrows.ForEach(x => x._page = _page);
    }
}
