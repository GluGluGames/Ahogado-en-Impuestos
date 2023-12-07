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
        public bool ableToPick = false;
        public bool endedRecollection = false;
        public float recollectionTime = 0;
        private float _pickingTimeDone = 0;
        

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

            if(ableToPick)
            {
                if(FillBar.fillAmount < 1)
                {
                    FillBar.fillAmount = _pickingTimeDone / recollectionTime;
                    _pickingTimeDone += Time.deltaTime;
                }
                else
                {
                    endedRecollection = true;
                }
            }
        }

        public void StopPicking()
        {
            ableToPick = false;
            _pickingTimeDone = 0;
            endedRecollection = false;
            recollectionTime = 0;
        }
    }
}