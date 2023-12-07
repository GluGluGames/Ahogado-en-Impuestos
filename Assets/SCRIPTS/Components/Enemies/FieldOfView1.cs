using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class FieldOfViewUpgraded : MonoBehaviour
    {
        public float radius;

        [Range(0, 360)]
        public float angle;

        [HideInInspector] public GameObject playerRef;
        
        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public bool imBlinded = false;

        public Action<List<Transform>> onGainDetection = (trans) => { };
        public Action onLostDetection = () => { };

        [HideInInspector] public List<Transform> seeing = new List<Transform>();

        private void Start()
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");        
        }

        public List<Transform> FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
            seeing.Clear();

            if (imBlinded) return seeing;

            if (rangeChecks.Length != 0)
            {
                foreach (Collider target in rangeChecks)
                {
                    Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

                    if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                        {
                            seeing.Add(target.transform);
                        }
                    }
                }
            }

            return seeing;
        }
    }
}