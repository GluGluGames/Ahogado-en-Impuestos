using System;
using UnityEngine;

namespace GGG.Components.UI.TileClean
{
    public class TileCleanCloseButton : MonoBehaviour
    {
        public Action OnCloseButton;

        public void OnClose() => OnCloseButton?.Invoke();
    }
}
