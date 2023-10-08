using GGG.Shared;

using System.Collections.Generic;
using UnityEngine;
using System;

namespace GGG.Components.Player
{
    public class PlayerManager : MonoBehaviour {
        #region Singleton

        public static PlayerManager Instance;

        private void Awake() {
            if (Instance != null) return;

            Instance = this;
        }

        #endregion

        private Dictionary<string, int> _resources = new();

        private void Start()
        {
            foreach(string i in Enum.GetNames(typeof(Resources))) {
                _resources.Add(i, 0);
            }
        }

        public int GetSeaweedsCount() { return _resources[Resources.SEAWEED.ToString()]; }

        public void AddResource(string resource, int amount) {
            if (!_resources.ContainsKey(resource)) {
                Debug.LogError("No resource found");
                return;
            }

            _resources[resource] += amount;
        }
    }
}
