using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Components.UI.Buttons;
using GGG.Components.UI.Containers;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallUi : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private List<GameObject> AchievementsPanels;
        [Space(5), Header("Buttons")]
        [SerializeField] private Button UpArrow;
        [SerializeField] private Button DownArrow;
        [SerializeField] private Button CloseButton;

        private GameManager _gameManager;
        private AchievementsManager _achievementsManager;

        private List<ContainerButton> _containerButtons;
        private GameObject _viewport;
        private int _currentPage;
        private int _currentAchievement;
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

            UpArrow.onClick.AddListener(() => ChangeAchievements(-1));
            DownArrow.onClick.AddListener(() => ChangeAchievements(1));
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void InitializeAchievements()
        {
            for (int i = 0; i < AchievementsPanels.Count; i++) 
                AchievementsPanels[i].SetActive(i == 0);

            _currentAchievement = 0;
            AchievementContainer[] containers = AchievementsPanels[0].GetComponentsInChildren<AchievementContainer>();
            
            foreach (AchievementContainer container in containers)
            {
                Achievement achievement = _achievementsManager.Achievement(_currentAchievement++);
                container.SetAchievement(achievement.GetName(), achievement.GetDescription(), achievement.GetSprite());
            }
        }

        private void ChangeAchievements(int direction)
        {
            if (_currentPage + direction >= AchievementsPanels.Count || _currentPage + direction <= 0) 
                return;

            AchievementsPanels[_currentPage].SetActive(false);
            int idx = _currentPage + direction;
            AchievementsPanels[idx].SetActive(true);

            AchievementContainer[] containers = AchievementsPanels[idx].GetComponentsInChildren<AchievementContainer>();

            foreach (AchievementContainer container in containers)
            {
                container.SetAchievement("", "", null);
            }

            _currentPage += direction;
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
            OnCityHallOpen?.Invoke();
            _gameManager.OnUIOpen();
            _open = true;
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            _viewport.SetActive(false);

            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _viewport.SetActive(false);
                _open = false;
                _gameManager.OnUIClose();
            };
        }
    }
}
