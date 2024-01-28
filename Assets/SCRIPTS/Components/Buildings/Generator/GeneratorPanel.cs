using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorPanel : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;

        private List<GeneratorLevelButton> _buttons = new();
        private GameObject _viewport;

        private void Awake()
        {
            _buttons = FindObjectsOfType<GeneratorLevelButton>(true).ToList();
            _viewport = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            if (Index != 0) Hide();
            else Show();
            
            _buttons.ForEach(x => x.OnButton += OnLevelButton);
        }

        private void OnDisable()
        {
            _buttons.ForEach(x => x.OnButton -= OnLevelButton);
        }

        private void OnLevelButton(int idx)
        {
            if (idx != Index)
            {
                Hide();
                return;
            }
            
            Show();
        }

        private void Show()
        {
            _viewport.SetActive(true);
        }

        private void Hide()
        {
            _viewport.SetActive(false);
        }

    }
}
