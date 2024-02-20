using System;
using GGG.Classes.Buildings;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Generator
{
    [RequireComponent(typeof(Image), typeof(CanvasGroup), typeof(Button))]
    public class GeneratorBoostButton : MonoBehaviour
    {
        [SerializeField, Range(1, 3)] private int Level;
        [SerializeField] private int Index;
        [SerializeField] private Sprite ActiveSprite;

        private Image _icon;
        private CanvasGroup _group;
        private BuildingComponent _building;
        private Generator _generator;

        private bool _shown;

        public Action OnBoost;

        private void Awake()
        {
            _icon = GetComponent<Image>();
            _icon.sprite = ActiveSprite;

            _group = GetComponent<CanvasGroup>();
        }

        private void OnValidate()
        {
            if (!ActiveSprite) return;

            _icon = GetComponent<Image>();
            _icon.sprite = ActiveSprite;
        }

        private void OnEnable()
        {
            GeneratorUI.OnGeneratorOpen += OnOpen;
            GeneratorUI.OnGeneratorClose += OnClose;
        }

        private void OnClose()
        {
            GeneratorUI.OnGeneratorClose -= OnClose;

            _generator = null;
            _building = null;
        }

        private void OnOpen(Generator generator)
        {
            _generator = generator;

            if (!_shown) Hide();
        }

        public void Initialize(BuildingComponent building)
        {
            _building = building;
            if (!building.IsBoost())
            {
                Hide();
                return;
            }

            Show();
        }

        public void OnBuildingBoost()
        {
            if (!_building || GameManager.Instance.TutorialOpen()) return;
            
            if (_building.IsBoost())
            {
                _building.EndBoost();
                _generator.AddGeneration(Level, -1);
                _generator.SetIndex(Level, Index, 0);
                _generator.SetBoostBuilding(Level, Index, -1);
                Hide();
                OnBoost?.Invoke();
                return;
            }

            if (_generator.CurrentGeneration(Level) >= Level) return;
            
            _building.Boost();
            _generator.AddGeneration(Level, 1);
            _generator.SetIndex(Level, Index, 1);
            _generator.SetBoostBuilding(Level, Index, _building.Id());
            Show();
            OnBoost?.Invoke();
        }

        private void Hide()
        {
            _group.alpha = 0;
            _shown = false;
        }

        private void Show() 
        {
            _group.alpha = 1;
            _shown = true;
        }
        
    }
}
