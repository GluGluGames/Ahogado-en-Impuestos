using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Generator
{
    [RequireComponent(typeof(Button))]
    public class GeneratorLevelButton : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;

        private Generator _generator;
        
        public Action<int> OnButton;

        private void OnEnable()
        {
            GeneratorUI.OnGeneratorOpen += OnOpen;
            GeneratorUI.OnGeneratorClose += OnClose;
            
            if (Index != 0) return;
            
            OnButtonPress();
        }

        public void OnButtonPress()
        {
            OnButton?.Invoke(Index);
        }

        private void OnOpen(Generator generator)
        {
            _generator = generator;
            if (_generator.CurrentLevel() < Index + 1)
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
        }

        private void OnClose()
        {
            GeneratorUI.OnGeneratorClose -= OnClose;
        }
    }
}
