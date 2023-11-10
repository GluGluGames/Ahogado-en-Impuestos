using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Tutorial;
using UnityEngine;

namespace GGG.Components.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private List<TutorialBase> Tutorials;

        private void OnValidate()
        {
            Tutorials = Resources.LoadAll<TutorialBase>("Tutorials").ToList();
        }
    }
}
