using System;
using System.Collections.Generic;
using GGG.Components.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Buildings
{
    public class BuildCost : MonoBehaviour
    {
        private BuildButton _button;
        private readonly List<TMP_Text> _costTexts = new();
        private readonly List<Image> _costImages = new();
        
        private void OnEnable()
        {
            if (!_button) _button = GetComponentInParent<BuildButton>();
            if (_costTexts.Count <= 0 && _costImages.Count <= 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    _costTexts.Add(transform.GetChild(i).GetComponentInChildren<TMP_Text>());
                    _costImages.Add(transform.GetChild(i).GetComponentInChildren<Image>());
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (i >= _button.Building().GetBuildingCost().GetCostsAmount() || !BuildingListener.CanBuild(_button.Building()))
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                    continue;
                }

                _costTexts[i].SetText(BuildingManager.Instance.GetBuildingCost(_button.Building()).GetCost(i).ToString());
                _costImages[i].sprite = _button.Building().GetBuildingCost().GetResource(i).GetSprite();
            }
        }
    }
}
