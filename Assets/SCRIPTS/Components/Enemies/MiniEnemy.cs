using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class MiniEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public MiniEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int stamina = 2;
        [SerializeField] private int staminaRechargeTime = 2;
        [SerializeField] private EnemyStateUI StateUI;
        private int tiredness = 0;

        private HexTile NotifiedHex;
        private List<HexTile> NotifiedHexPath;
        private float WalkDelta = 0f;
        private bool OnBT = false;
        private bool SeePlayer = false;
        [SerializeField] private int SearchPatience = 4;
        private float SearchDelta = 0f;
        private bool CanEnterBT = true;
        private bool Notified = false;

        private void Awake()
        {
            fov.onGainDetection += (trans) =>
            {
                if (trans.tag == "Player")
                {
                    if (!OnBT)
                    {
                        enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                        ai.playerDetectedPush.Fire();
                    }
                    else
                    {
                        SeePlayer = true;
                    }
                }
                else if (trans.GetComponent<EnemyComponent>() != null)
                {
                    EnemyComponent aux = trans.GetComponent<EnemyComponent>();
                    if (aux.size > enemyComp.size)
                    {
                        enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                        ai.detectBiggerEnemyPush.Fire();
                    }
                }
            };

            fov.onLostDetection += () =>
            {
                if (!OnBT)
                {
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    ai.lostPatiencePush.Fire();
                }
                else
                {
                    SeePlayer = false;
                }
            };

            ai.StartPatrol += () =>
            {
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
                Notified = false;
                CanEnterBT = true;
                StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();

                if (Notified) { return BehaviourAPI.Core.Status.Success; }
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += CountTiredness;
                StateUI.ChangeState(StateIcon.ChasingState);
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.SleepMethod += () =>
            {
                enemyComp.movementController.onMove -= CountTiredness;
                enemyComp.movementController.movingAllowed = false;
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StateUI.ChangeState(StateIcon.SleepState);
                StartCoroutine(OnSleepCoroutine());
            };

            ai.FleeMethod += () =>
            {
                enemyComp.movementController.onMove -= CountTiredness;
                StateUI.ChangeState(StateIcon.FleeState);
                HexTile destination = enemyComp.movementController.Flee(3);
                StartCoroutine(CheckFleeingStatus(destination));
            };

            ai.UpdateCheckOnDestination += () =>
            {
                if (enemyComp.movementController.GetCurrentTile() == NotifiedHex || NotifiedHexPath.Count == 0)
                {
                    return BehaviourAPI.Core.Status.Success;
                }
                else
                {
                    return BehaviourAPI.Core.Status.Failure;
                }
            };

            ai.UpdateChaseExit += () =>
            {
                ai.EnemyFoundWhileBTPush.Fire();
                return BehaviourAPI.Core.Status.Success;
            };

            ai.UpdateMoveClose += () =>
            {

                if (SearchDelta >= enemyComp.movementController.ticker.tickTime)
                {

                    HexTile GoTo = Pathfinder.GetWalkableRandomNeighbour(enemyComp.movementController.GetCurrentTile());

                    enemyComp.movementController.SetCurrentTile(GoTo);
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    enemyComp.movementController.MoveTo(new Vector3(GoTo.transform.position.x, GoTo.transform.position.y + 1, GoTo.transform.position.z));
                    SearchDelta = 0f;
                    SearchPatience -= 1;

                    return BehaviourAPI.Core.Status.Success;
                }

                SearchDelta += Time.deltaTime;
                StateUI.ChangeState(StateIcon.MovingCloseState);

                return BehaviourAPI.Core.Status.Success;
            };

            ai.UpdatePatrolExit += () =>
            {
                ai.EnemyNotFoundWhileBTPush.Fire();
                return BehaviourAPI.Core.Status.Success;
            };

            ai.UpdateWalkToDestination += () =>
            {
                if (NotifiedHexPath.Count == 0) return BehaviourAPI.Core.Status.Success;

                if (WalkDelta >= enemyComp.movementController.ticker.tickTime)
                {
                    WalkDelta = 0f;

                    enemyComp.movementController.SetCurrentTile(NotifiedHexPath[0]);
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    enemyComp.movementController.MoveTo(NotifiedHexPath[0].transform.position + new Vector3(0, 1, 0));
                    NotifiedHexPath.RemoveAt(0);

                    return BehaviourAPI.Core.Status.Success;
                }

                WalkDelta += Time.deltaTime;
                StateUI.ChangeState(StateIcon.MovingAtHexState);
                return BehaviourAPI.Core.Status.Failure;
            };

            ai.ConditionSeePlayerCheck += () =>
            {
                return SeePlayer;
            };

            ai.ConditionKeepSearchingCheck += () =>
            {
                return SearchPatience > 0;
            };

        }

        private void CountTiredness()
        {
            tiredness++;
            if (tiredness == stamina)
            {
                StartCoroutine(OnStaminaRecharge());
                tiredness = 0;
            }
        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(enemyComp.restTime);
            enemyComp.movementController.movingAllowed = true;
            ai.restedPush.Fire();
        }

        private IEnumerator CheckFleeingStatus(HexTile destinationCheck)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                enemyComp.currentTile = enemyComp.movementController.currentTile;
                if (enemyComp.movementController.currentTile.name.Equals(destinationCheck.name))
                {
                    break;
                }
            }
            ai.distantPush.Fire();
        }

        private IEnumerator OnStaminaRecharge()
        {
            StateIcon currStateIcon = StateUI.GetCurrentState();
            StateUI.ChangeState(StateIcon.RestState);

            fov.imBlinded = true;
            enemyComp.movementController.movingAllowed = false;
            yield return new WaitForSeconds(staminaRechargeTime);
            fov.imBlinded = false;
            enemyComp.movementController.movingAllowed = true;

            StateUI.ChangeState(currStateIcon);
        }

        public void GetNotified(HexTile hexToGo)
        {
            if (OnBT && !CanEnterBT) return;

            Notified = true;
            OnBT = true;
            enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
            enemyComp.movementController.currentPath.Clear();
            NotifiedHexPath = Pathfinder.FindPath(enemyComp.currentTile, hexToGo);
            NotifiedHexPath.Reverse();

            if (NotifiedHexPath.Count != 0) { NotifiedHexPath.RemoveAt(0); }
        }
    }
}