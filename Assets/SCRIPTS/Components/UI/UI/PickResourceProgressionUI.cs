using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class PickResourceProgressionUI : MonoBehaviour
    {
        
        [SerializeField] private Image FillBar;
        
        private Camera _camera;

        private void OnEnable()
        {
            FillBar.fillAmount = 0;
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            transform.LookAt(transform.position + _camera.transform.rotation * Vector3.back, 
                _camera.transform.rotation * Vector3.up);
        }

        public IEnumerator PickResource(int recollectionTime)
        {
            float aux = 0;

            while (FillBar.fillAmount < 1)
            {
                FillBar.fillAmount = aux / recollectionTime;
                aux += Time.deltaTime;
                yield return null;
            }
        }

        public void StopPicking() => StopAllCoroutines();
    }
}