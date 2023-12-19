using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class BerskerEnemy : MonoBehaviour
    {
        public FieldOfViewUpgraded fov;
        public BerserkerEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int maxPatience = 2;
        [SerializeField] private EnemyStateUI StateUI;
        private int patience = 0;
        private float delta = 0;
        public float visionFrequency;
        private List<Transform> transformsSeen;
        private Transform target;
        private bool targetOnVision;
        private bool berserkerMode = false;

        private void Awake()
        {
            ai.StartPatrol += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                target = GameObject.FindWithTag("Player").transform;
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
                StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                if (targetOnVision)
                {
                    ai.detectPlayerPush.Fire();
                }

                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += countPatience;
                StateUI.ChangeState(StateIcon.ChasingState);
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(target.transform.position);
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                if (transformsSeen.Count == 0) ai.lostVisionPush.Fire();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartBerserker += () =>
            {
                updateTargetTile();
                enemyComp.movementController.gotPath = false;
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                berserkerMode = true;
                fov.imBlinded = false;
                StateUI.ChangeState(StateIcon.BerserkerState);
            };

            ai.UpdateBerserker += () =>
            {
                transform.LookAt(target.transform.position);
                updateTargetTile();
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdateBerserker();
                if (transformsSeen.Count == 0) ai.killedNonPlayerPush.Fire();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.SleepMethod += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.movingAllowed = false;
                enemyComp.movementController.onMove -= countPatience;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
                StateUI.ChangeState(StateIcon.SleepState);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject.layer != LayerMask.NameToLayer("Enemy") 
                || other.transform.gameObject.layer != LayerMask.NameToLayer("Berserker") 
                || other.transform.gameObject.layer != LayerMask.NameToLayer("JunkDealer") 
                || other.transform.gameObject.layer != LayerMask.NameToLayer("EnemySentinel")) return;
        }

        private void checkVision()
        {
            if (target == null) return;
            if (transformsSeen.Count == 0) targetOnVision = false;
            chooseTarget();
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

        private void chooseTarget()
        {
            if (transformsSeen.Count == 0) return;
            if (berserkerMode)
            {
                Transform closestTarget = null;
                float closestDistance = 999999999999999999;
                foreach (Transform t in transformsSeen)
                {
                    float distance = Vector3.Distance(t.position, transform.position);
                    if (distance < closestDistance)
                    {
                        closestTarget = t;
                        closestDistance = distance;
                    }
                }
                target = closestTarget;
            }
            else
            {
                target = GameObject.FindWithTag("Player").transform;
            }
        }

        private void updateTargetTile()
        {
            if (!targetOnVision) { return; }
            if (target.tag == "Player")
            {
                enemyComp.movementController.targetTile = PlayerPosition.CurrentTile;
            }
            else
            {
                enemyComp.movementController.targetTile = target.GetComponent<EnemyComponent>().currentTile;
            }
        }

        private void countPatience()
        {
            patience++;
            if (patience == maxPatience)
            {
                //StartCoroutine(OnStaminaRecharge());
                ai.lostPatiencePush.Fire();
                patience = 0;
            }
        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(enemyComp.restTime);
            enemyComp.movementController.movingAllowed = true;
            ai.restedPush.Fire();
        }
    }
}