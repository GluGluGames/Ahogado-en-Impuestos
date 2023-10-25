using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Ticks
{
    public class TickManager : MonoBehaviour
    {

        #region singleton
        public static TickManager Instance;
        #endregion
        /// <summary>
        /// Action that ocurs every second
        /// </summary>
        public static Action OnTick;

        // Start is called before the first frame update
        private void Start()
        {
            Instance = this;
            StartCoroutine(Tick());
        }

        /// <summary>
        /// Each seconds there is a game Tick. OnTick.Invoke is called every time
        /// </summary>
        /// <returns></returns>
        private IEnumerator Tick()
        {
            OnTick?.Invoke();
            yield return new WaitForSeconds(1f);
            StartCoroutine(Tick());
        }

        public IEnumerator WaitSeconds(int currentSeconds, int maxSeconds, Action onEachSecond, Action onEnd)
        {
            currentSeconds++;
            yield return new WaitForSeconds(1);
            onEachSecond.Invoke();
            if (currentSeconds < maxSeconds)
            {
                StartCoroutine(WaitSeconds(currentSeconds, maxSeconds, onEachSecond, onEnd));
            }
            else
            {
                onEnd.Invoke();
            }
        }
    }
}