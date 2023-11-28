using GGG.Components.HexagonalGrid;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class BerskerEnemy : MonoBehaviour
    {
        public FieldOfViewUpgraded fov;
        public BerserkerEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int stamina = 2;
        [SerializeField] private int staminaRechargeTime = 2;
        private int tiredness = 0;
        private float delta = 0;
        public float visionFrequency;
        private List<Transform> transformsSeen;
        private Transform target;
        private bool targetOnVision;

        private void Awake()
        {
            ai.StartPatrol += () =>
            {
                target = GameObject.FindWithTag("Player").transform;
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                if (targetOnVision)
                {
                    Debug.Log("ey");
                    ai.detectPlayerPush.Fire();
                }

                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                //enemyComp.movementController.onMove += countTiredness;
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartBerserker += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                //enemyComp.movementController.onMove += countTiredness;
            };

            ai.UpdateBerserker += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.SleepMethod += () =>
            {
                enemyComp.movementController.movingAllowed = false;
                enemyComp.movementController.onMove -= countTiredness;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
            };
        }

        private void Start()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        private void Update()
        {
            delta += Time.deltaTime;

            if (delta >= visionFrequency)
            {
                delta = 0;
                transformsSeen = fov.FieldOfViewCheck();
                checkVision();
            }
        }

        private void checkVision()
        {
            if (target == null) return;
            if (transformsSeen.Count == 0) targetOnVision = false;
            foreach (Transform t in transformsSeen)
            {
                if (t.name == target.name)
                {
                    targetOnVision = true;
                }
                else
                {
                    targetOnVision = false;
                }
            }
        }

        private void countTiredness()
        {
            tiredness++;
            if (tiredness == stamina)
            {
                StartCoroutine(onStaminaRecharge());
                tiredness = 0;
            }
        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(2.0f);
            enemyComp.movementController.movingAllowed = true;
        }

        private IEnumerator onStaminaRecharge()
        {
            fov.imBlinded = true;
            enemyComp.movementController.movingAllowed = false;
            yield return new WaitForSeconds(staminaRechargeTime);
            fov.imBlinded = false;
            enemyComp.movementController.movingAllowed = true;
        }
    }
}