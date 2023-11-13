using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class FieldOfView : MonoBehaviour
    {
        public float radius;

        [Range(0, 360)]
        public float angle;

        public GameObject playerRef;

        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public bool canSeePlayer;
        public bool imBlinded = false;

        public Action onGainDetection = () => { };
        public Action onLostDetection = () => { };

        private void Start()
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FOVRoutine());
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (rangeChecks.Length != 0 && !imBlinded)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        if (!canSeePlayer) { onGainDetection.Invoke(); }
                        canSeePlayer = true;
                    }
                    else
                    {
                        if (canSeePlayer) { onLostDetection.Invoke(); }
                        canSeePlayer = false;
                    }
                }
                else
                {
                    if (canSeePlayer) { onLostDetection.Invoke(); }
                    canSeePlayer = false;
                }
            }
            else if (canSeePlayer || imBlinded)
            {
                if (canSeePlayer) { onLostDetection.Invoke(); }
                canSeePlayer = false;
            }
        }
    }
}