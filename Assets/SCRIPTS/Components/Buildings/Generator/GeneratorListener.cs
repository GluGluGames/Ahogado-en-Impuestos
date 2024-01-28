using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.HexagonalGrid;
using UnityEngine;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorListener : MonoBehaviour
    {
        private List<GeneratorBoostText> _texts;
        private List<GeneratorBoostButton> _buttons;
        private static Generator _currentGenerator;
        private HexTile _tile;

        private void Awake()
        {
            _texts = GetComponentsInChildren<GeneratorBoostText>(true).ToList();
            _buttons = GetComponentsInChildren<GeneratorBoostButton>(true).ToList();
        }

        private void OnEnable()
        {
            GeneratorUI.OnGeneratorOpen += OnOpen;
            GeneratorUI.OnGeneratorClose += OnClose;
        }

        private void OnDisable()
        {
            GeneratorUI.OnGeneratorOpen -= OnOpen;
            GeneratorUI.OnGeneratorClose -= OnClose;
        }

        private void OnOpen(Generator generator)
        {
            _currentGenerator = generator;
            _tile = TileManager.Instance.GetHexTiles()
                .Find(x => x.GetCurrentBuilding() && x.GetCurrentBuilding().Equals(generator));
            
            Initialize();
        }

        private void OnClose()
        {
            _currentGenerator = null;
            _tile = null;
        }

        private void Initialize()
        {
            int idx = 0;
            
            foreach (HexTile tile in _tile.neighbours)
            {
                if (!tile.GetCurrentBuilding() || !tile.GetCurrentBuilding().BuildData().CanBeBoost())
                {
                    idx++;
                    continue;
                }
                
                _texts[idx].InitializeText(tile.GetCurrentBuilding().BuildData());
                _texts[idx + 6].InitializeText(tile.GetCurrentBuilding().BuildData());
                _texts[idx + 12].InitializeText(tile.GetCurrentBuilding().BuildData());
                
                _buttons[idx].Initialize(tile.GetCurrentBuilding());
                _buttons[idx + 6].Initialize(tile.GetCurrentBuilding());
                _buttons[idx + 12].Initialize(tile.GetCurrentBuilding());
                
                idx++;
            }
        }
    }
}
