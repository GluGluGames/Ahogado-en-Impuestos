using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Ticks
{
    public class TickManager : MonoBehaviour
    {
        /// <summary>
        /// Action that ocurs every second
        /// </summary>
        public static Action OnTick;

        // Start is called before the first frame update
        private void Start()
        {
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
    }
}