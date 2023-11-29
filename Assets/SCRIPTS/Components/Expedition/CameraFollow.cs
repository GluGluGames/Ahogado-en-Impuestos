using System;
using UnityEngine;

namespace GGG.Components.Expedition
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform FollowTarget;
        [SerializeField] private float SmoothTime;

        private Vector3 _offset;
        private Vector3 _currentVelocity = Vector3.zero;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _offset = _transform.position + _offset;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = FollowTarget.position + _offset;
            _transform.position =
                Vector3.SmoothDamp(_transform.position, targetPosition, ref _currentVelocity, SmoothTime);
        }
    }
}
