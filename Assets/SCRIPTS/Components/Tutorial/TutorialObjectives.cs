using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace GGG.Components.Tutorial
{
    public class TutorialObjectives : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> Texts;
        
        private GameObject _viewport;

        private void Awake()
        {
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        public void SetObjectives(TutorialObjective objective)
        {
            for (int i = 0; i < objective.GetObjectives().Count; i++)
            {
                Texts[i].gameObject.SetActive(true);
                Texts[i].SetText(objective.Objective(i));
            }
        }

        public void Show() => _viewport.SetActive(true);
        public void Hide()
        {
            Texts.ForEach(x => x.gameObject.SetActive(false));
            _viewport.SetActive(false);
        }
    }
}
