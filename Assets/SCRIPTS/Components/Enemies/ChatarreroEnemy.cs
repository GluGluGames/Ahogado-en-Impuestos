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
        private bool IgnoringPlayer = true;
        private bool ChasingPlayer = false;

        private void Awake()
        {
            ai.StartPatrol += () =>
            {
                Debug.Log("Patruyo");
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
                //enemyComp.movementController.onMove += countPatience;
                CountingPatience = true;
                //StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                if (targetOnVision)
                {
                    if (target.CompareTag("Player"))
                    {
                        // transicion a chase player
                    }
                    else
                    {
                        IgnoringPlayer = true;
                        ChasingPlayer = false;
                        ai.DetectedBetterResourcePush.Fire();
                        //return BehaviourAPI.Core.Status.Failure;
                    }
                }

                    if (Tiredness >= MaxTiredness)
                    {
                        Tiredness = 0;
                        return BehaviourAPI.Core.Status.Success;
                    }
                    else
                    {
                        return BehaviourAPI.Core.Status.Running;
                    }
                
            };

            ai.StartTaking += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                //enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                if (!CountingPatience)
                {
                    //enemyComp.movementController.onMove += countPatience;
                }

                //StateUI.ChangeState(StateIcon.ChasingState);
            };

            ai.UpdateTaking += () =>
            {
                transform.LookAt(target.transform.position);
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdateJunkDealer();
                //if (transformsSeen.Count == 0) ai.lostVisionPush.Fire();

                updateTargetTile();
                Debug.Log(target);
                Debug.Log(enemyComp.movementController.targetTile.name);
                //if (Tiredness >= MaxTiredness)
                //{
                //    Tiredness = 0;
                //    return BehaviourAPI.Core.Status.Success;
                //}
                //else
                //{
                //    return BehaviourAPI.Core.Status.Running;
                //}

                return BehaviourAPI.Core.Status.Running;
            };

            //ai.StartStealing += () =>
            //{
            //    updateTargetTile();
            //    enemyComp.movementController.gotPath = false;
            //    enemyComp.movementController.currentPath.Clear();
            //    enemyComp.movementController.imChasing = true;
            //    berserkerMode = true;
            //    fov.imBlinded = false;
            //    StateUI.ChangeState(StateIcon.BerserkerState);
            //};

            //ai.UpdateTaking += () =>
            //{
            //    transform.LookAt(target.transform.position);
            //    updateTargetTile();
            //    if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdateBerserker();
            //    if (transformsSeen.Count == 0) ai.killedNonPlayerPush.Fire();
            //    return BehaviourAPI.Core.Status.Running;
            //};

            ai.UpdateSleep += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.movingAllowed = false;
                enemyComp.movementController.onMove -= countPatience;
                fov.imBlinded = true;
                //StateUI.ChangeState(StateIcon.SleepState);

                if (deltaSleep >= staminaRechargeTime)
                {
                    enemyComp.movementController.movingAllowed = true;
                    deltaSleep = 0f;
                    Debug.Log("regreso");
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
        }

        private void checkVision()
        {
            if (transformsSeen.Count == 0) targetOnVision = false;
            ChooseTarget();
            if (target == null) return;

            foreach (Transform t in transformsSeen)
            {
                if (t.name == target.name)
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

            enemyComp.movementController.chasingPlayer = bestTarget.CompareTag("Player");

            target = bestTarget;
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

        private void updateTargetTile()
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

        private void countPatience()
        {
            Tiredness++;
            Debug.Log(Tiredness);
            //if (Tiredness >= MaxTiredness)
            //{
            //    ai.LostStaminaPatrollingPush.Fire();
            //    Tiredness = 0;
            //}
        }
    }
}