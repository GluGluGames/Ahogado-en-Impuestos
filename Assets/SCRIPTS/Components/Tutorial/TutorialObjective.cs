using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace GGG.Components.Tutorial
{
    [System.Serializable]
    public class TutorialObjective
    {
        [SerializeField] private List<LocalizedString> Objectives;

        public string Objective(int idx) => Objectives[idx].GetLocalizedString();
        public List<LocalizedString> GetObjectives() => Objectives;
    }
}
