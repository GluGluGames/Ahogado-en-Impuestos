using UnityEngine;

namespace GGG.Shared {
    public static class Holding
    {
        private static bool _isHolding;

        public static bool IsHolding() { return _isHolding; }

        public static void IsHolding(bool holding) { _isHolding = holding; }
    }
}
