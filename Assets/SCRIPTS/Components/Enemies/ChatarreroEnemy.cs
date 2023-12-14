using GGG.Components.HexagonalGrid;
using GGG.Components.Resources;
using GGG.Components.UI;
using GGG.Shared;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class ChatarreroEnemy : MonoBehaviour
    {
        public FieldOfViewUpgraded fov;
        public ChatarreroEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int staminaRechargeTime = 2;
        [SerializeField] private EnemyStateUI StateUI;
        private float deltaVision = 0;
        private float deltaSleep = 0;
        public float visionFrequency;
        private List<Transform> transformsSeen;
        private Transform target;
        private bool targetOnVision;
        private int Tiredness;
        [SerializeField] private int MaxTiredness;

        private bool CountingPatience = false;
        private bool IgnoringPlayer = false;
        private bool ChasingPlayer = false;

        private Resource CurrentResource;
        private int ValueCurrentResource = -1;

        private bool TakingResource = false;
        private float DeltaTakingResource = 0f;
        private float TimeToTakeResource = 3f;

        private void Awake()
        {
            ai.StartPatrol += () =>
            {
                Debug.Log("Patruyo");
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;

                StartCountingPatience();

                CountingPatience = true;
                targetOnVision = false;
                //StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                if (targetOnVision)
                {
                    if (target.CompareTag("Player"))
                    {
                        ai.DetectedBetterResourceOnPlayerPush.Fire();
                    }
                    else
                    {
                        ai.DetectedBetterResourcePush.Fire();
                    }
                }

                return FinishUpdateOnCouningPatience();
            };

            ai.StartTaking += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                IgnoringPlayer = true;
                ChasingPlayer = false;
                fov.imBlinded = false;

                StartCountingPatience();

                if(CurrentResource != null)
                {
                    Debug.Log(ValueCurrentResource);
                    Debug.Log(($"VOY A POR RECURSO, TENGO: {CurrentResource.name}"));
                }

                //StateUI.ChangeState(StateIcon.ChasingState);
            };

            ai.UpdateTaking += () =>
            {
                if (target == null)
                {
                    Debug.Log("target NULL");
                    targetOnVision = false;
                    ai.ResourceNotOnSightPush.Fire();
                }

                transform.LookAt(target.transform.position);
                UpdateTargetTile();
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdateJunkDealer();

                return FinishUpdateOnCouningPatience();
            };

            ai.StartStealing += () =>
            {
                UpdateTargetTile();
                enemyComp.movementController.gotPath = false;
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;

                fov.imBlinded = false;
                //StateUI.ChangeState(StateIcon.BerserkerState);
            };

            ai.UpdateStealing += () =>
            {
                transform.LookAt(target.transform.position);
                UpdateTargetTile();
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdateJunkDealer();
                if (!target.CompareTag("Player"))
                    ai.DetectedBetterResourceOnMapPush.Fire();

                return FinishUpdateOnCouningPatience();
            };

            ai.UpdateSleep += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.movingAllowed = false;
                StopCountingPatience();
                fov.imBlinded = true;
                //StateUI.ChangeState(StateIcon.SleepState);

                if (deltaSleep >= staminaRechargeTime)
                {
                    enemyComp.movementController.movingAllowed = true;
                    deltaSleep = 0f;
                    return BehaviourAPI.Core.Status.Success;
                }
                else
                {
                    deltaSleep += Time.deltaTime;
                    return BehaviourAPI.Core.Status.Running;
                }
            };
        }

        private void Start()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        private void Update()
        {
            deltaVision += Time.deltaTime;

            if (deltaVision >= visionFrequency)
            {
                deltaVision = 0;
                transformsSeen = fov.FieldOfViewCheck();
                checkVision();
            }

            if (TakingResource)
            {
                DeltaTakingResource += Time.deltaTime;
                if (DeltaTakingResource >= TimeToTakeResource)
                {
                    DeltaTakingResource = 0;
                    TakeResourceOnMap();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform != target) return;

            if (other.transform.CompareTag("Player"))
            {
            }
            else
            {
                TakingResource = true;
            }
        }

        private BehaviourAPI.Core.Status FinishUpdateOnCouningPatience()
        {
            if (Tiredness >= MaxTiredness)
            {
                Tiredness = 0;
                return BehaviourAPI.Core.Status.Success;
            }
            else
            {
                return BehaviourAPI.Core.Status.Running;
            }
        }

        private void StartCountingPatience()
        {
            if (CountingPatience) return;

            CountingPatience = true;
            enemyComp.movementController.onMove += CountPatience;
        }

        private void StopCountingPatience()
        {
            if (!CountingPatience) return;

            CountingPatience = false;
            enemyComp.movementController.onMove -= CountPatience;
        }

        private void checkVision()
        {
            if (transformsSeen.Count == 0) targetOnVision = false;
            ChooseTarget();
            if (target == null)
            {
                targetOnVision = false;
                return;
            }

            foreach (Transform t in transformsSeen)
            {
                if (t == target)
                {
                    targetOnVision = true;
                    break;
                }
                else
                {
                    targetOnVision = false;
                }
            }
        }

        private void ChooseTarget()
        {
            if (transformsSeen.Count == 0) return;

            Transform bestTarget = null;
            int bestValue = -1;
            foreach (Transform t in transformsSeen)
            {
                if (t.gameObject.tag == "Player" && (!IgnoringPlayer))
                {
                    // Check if the player has a good enough resource

                    int bestResPlayer = CheckPlayerInventory() != null ? CheckPlayerInventory().GetResourceValue() : -1;
                    if (bestResPlayer > bestValue)
                    {
                        bestValue = bestResPlayer;
                        bestTarget = t;
                    }
                }
                else if (IgnoringPlayer || !ChasingPlayer)
                {
                    //ResourceComponent resComp;
                    if (t.TryGetComponent<ResourceComponent>(out ResourceComponent resComp) && resComp != null && resComp.GetResource().GetResourceValue() > bestValue)
                    {
                        bestValue = resComp.GetResource().GetResourceValue();
                        bestTarget = t;
                    }
                }
            }

            if (bestTarget != null)
            {
                enemyComp.movementController.chasingPlayer = bestTarget.CompareTag("Player");
            }

            if (bestValue > ValueCurrentResource)
            {
                target = bestTarget;

            }
        }

        private Resource CheckPlayerInventory()
        {
            int bestValue = -1;
            Resource bestComp = null;
            List<Resource> list = new List<Resource>();

            foreach (var r in ResourceManager.Instance.resourcesCollected.ToList())
            {
                if (r.Value > 0)
                {
                    list.Add(r.Key);
                }
            }

            foreach (Resource res in list)
            {
                if (res.GetResourceValue() > bestValue)
                {
                    bestValue = res.GetResourceValue();
                    bestComp = res;
                }
            }

            return bestComp;
        }

        private void UpdateTargetTile()
        {
            if (!targetOnVision) { return; }

            if (target.CompareTag("Player"))
            {
                enemyComp.movementController.targetTile = PlayerPosition.CurrentTile;
            }
            else
            {
                enemyComp.movementController.targetTile = target.GetComponent<ResourceComponent>().currentTile;
            }
        }

        private void CountPatience()
        {
            Tiredness++;
            Debug.Log(Tiredness);
        }

        private void TakeResourceOnMap()
        {
            TakingResource = false;

            // Update UI or smth
            CurrentResource = target.GetComponent<ResourceComponent>().GetResource();
            ValueCurrentResource = CurrentResource.GetResourceValue();

            target.gameObject.SetActive(false);
            target = null;

            targetOnVision = false;
            ai.ResourceCollectedPush.Fire();
        }
    }
}