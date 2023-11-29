using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Ticks
{
    public class Ticker : MonoBehaviour
    {
        public float tickTime = 1f;
        public Action onTick = () => { };
        public bool keepTicking = true;

        // Start is called before the first frame update
        private void Awake()
        {
            StartCoroutine(ticker());
        }

        private void OnDisable()
        {
            StopCoroutine(ticker());
        }

        private void OnDestroy()
        {
            StopCoroutine(ticker());
        }

        private IEnumerator ticker()
        {
            while (keepTicking)
            {
                yield return new WaitForSeconds(tickTime);
                onTick.Invoke();
            }
        }
    }
}