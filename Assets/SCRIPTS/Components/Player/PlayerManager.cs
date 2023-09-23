using System;
using UnityEngine;

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

        private int _seaweeds;
        
        public int GetSeaweedsCount() { return _seaweeds; }

        public void AddSeaweeds(int count) { _seaweeds += count; }
    }
}
