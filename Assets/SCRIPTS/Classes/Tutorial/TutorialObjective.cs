using UnityEngine;
using UnityEngine.Localization;

namespace GGG.Classes.Tutorial
{
    [System.Serializable]
    public class TutorialObjective
    {
        [SerializeField] private LocalizedString[] Objectives;

        public LocalizedString[] GetObjectives => Objectives;
        public LocalizedString Objective(int idx) => Objectives[idx];
    }
}
