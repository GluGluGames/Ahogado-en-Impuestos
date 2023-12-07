using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Achievements;
using GGG.Components.Core;
using GGG.Components.Taxes;
using GGG.Components.UI.Buttons;
using GGG.Components.UI.Containers;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallUi : MonoBehaviour
    {
        [Header("Texts")] 
        [SerializeField] private TMP_Text PagesText;
        [SerializeField] private TMP_Text TaxesCounter;
        [Space(5), Header("Buttons")]
        [SerializeField] private Button UpArrow;
        [SerializeField] private Button DownArrow;
        [SerializeField] private Button CloseButton;

        private GameManager _gameManager;
        private AchievementsManager _achievementsManager;

        private List<ContainerButton> _containerButtons;
        private List<AchievementContainer> _achievementContainers;
        private GameObject _viewport;
        private int _currentPage = 1;
        private bool _open;

        public static Action OnCityHallOpen;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _achievementsManager = AchievementsManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            _viewport.SetActive(false);

            _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();
            for (int i = 0; i < _containerButtons.Count; i++)
            {
                _containerButtons[i].OnButtonClick +=
                    _containerButtons[(i + 1) % _containerButtons.Count].DeselectButton;
            }

            _currentPage = 1;
            _achievementContainers = GetComponentsInChildren<AchievementContainer>(true).ToList();
            PagesText.SetText($"{_currentPage}/{_achievementsManager.GetAchievements().Count / 3}");

            UpArrow.onClick.AddListener(() => ChangeAchievements(-1));
            DownArrow.onClick.AddListener(() => ChangeAchievements(1));
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void InitializeAchievements()
        {
            List<Achievement> achievements = _achievementsManager.GetAchievements();
            for (int i = 0; i < _achievementContainers.Count; i++)
            {
                _achievementContainers[i].SetAchievement(achievements[i].GetName(),
                    achievements[i].GetDescription(),
                    achievements[i].GetSprite());
            }
        }

        private void ChangeAchievements(int direction)
        {
            List<Achievement> achievements = _achievementsManager.GetAchievements();
            
            if ((_currentPage + direction) * 3 > achievements.Count || _currentPage + direction <= 0) 
                return;
            
            int idx = (_currentPage - 1 + direction) * 3;

            for (int i = 0; i < 3; i++)
            {
                _achievementContainers[i].SetAchievement(achievements[idx].GetName(), 
                    achievements[idx].GetDescription(), 
                    achievements[idx].GetSprite());
                idx++;
            }

            _currentPage += direction;
            PagesText.SetText($"{_currentPage}/{_achievementsManager.GetAchievements().Count / 3}");
        }

        private void InitializeTaxCounter()
        {
            float timerDelta = TaxManager.GetRemainingTime();
            int minutes = Mathf.FloorToInt(timerDelta / 60);
            int seconds = Mathf.FloorToInt(timerDelta % 60);
            TaxesCounter.SetText($"{minutes:00}:{seconds:00}");     
        }

        private IEnumerator TaxCounter()
        {
            int minutes, seconds;
            float timerDelta;
            
            while (_open)
            {
                timerDelta = TaxManager.GetRemainingTime();
                minutes = Mathf.FloorToInt(timerDelta / 60);
                seconds = Mathf.FloorToInt(timerDelta % 60);
                TaxesCounter.SetText($"{minutes:00}:{seconds:00}");
                yield return null;
            }
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen()) return;

            Close();
        }

        public void Open()
        {
            if (_open) return;
            _viewport.SetActive(true);
            
            foreach(ContainerButton button in _containerButtons)
                button.Initialize();
            
            InitializeAchievements();
            InitializeTaxCounter();
            OnCityHallOpen?.Invoke();
            _gameManager.OnUIOpen();
            _open = true;
            StartCoroutine(TaxCounter());
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            StopAllCoroutines();
            
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _viewport.SetActive(false);
                _open = false;
                _gameManager.OnUIClose();
            };
        }
    }
}
