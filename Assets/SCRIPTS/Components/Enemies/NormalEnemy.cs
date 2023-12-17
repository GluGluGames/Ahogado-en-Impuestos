using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class NormalEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public NormalEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int stamina;
        [SerializeField] private int staminaRechargeTime;
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
                        ai.playerDetected.Fire();
                    }
                    else
                    {
                        SeePlayer = true;
                    }
                }
            };

            fov.onLostDetection += () =>
            {
                if (!OnBT)
                {
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    ai.playerLost.Fire();
                }
                else
                {
                    SeePlayer = false;
                }
            };

            ai.StartPatrol += () =>
            {
                OnBT = false;
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
                Notified = false;
                CanEnterBT = true;

                StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();

                if(Notified) { return BehaviourAPI.Core.Status.Success; }
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                StateUI.ChangeState(StateIcon.ChasingState);
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                CanEnterBT = false;
                Notified = false;
                enemyComp.movementController.onMove += CountTiredness;
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.UpdateCheckOnDestination += () =>
            {
                Debug.Log("check destination");
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
                Debug.Log("exit to chase");
                ai.EnemyFoundWhileBTPush.Fire();
                return BehaviourAPI.Core.Status.None;
            };

            ai.UpdateMoveClose += () =>
            {
                Debug.Log("me desplazo cerca");

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

                return BehaviourAPI.Core.Status.Success;
            };

            ai.UpdatePatrolExit += () =>
            {
                Debug.Log("Go back to patrol");
                ai.EnemyNotFoundWhileBTPush.Fire();
                return BehaviourAPI.Core.Status.None;
            };

            ai.UpdateWalkToDestination += () =>
            {
                Debug.Log("Walk towards destination");
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
                return BehaviourAPI.Core.Status.Failure;
            };

            ai.ConditionSeePlayerCheck += () =>
            {
                Debug.Log("CheckSeeEnemy");
                return SeePlayer;
            };

            ai.ConditionKeepSearchingCheck += () =>
            {
                Debug.Log("Keep searching");
                return SearchPatience > 0;
            };

            ai.StartSleep += () =>
            {
                StateUI.ChangeState(StateIcon.SleepState);
                enemyComp.movementController.movingAllowed = false;
                enemyComp.movementController.onMove -= CountTiredness;
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
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
            yield return new WaitForSeconds(2.0f);
            enemyComp.movementController.movingAllowed = true;
            ai.RestedPush.Fire();
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
            Debug.Log("soy notificado");
            if (OnBT && !CanEnterBT) return;

            Notified = true;
            OnBT = true;
            enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
            enemyComp.movementController.currentPath.Clear();
            NotifiedHexPath = Pathfinder.FindPath(enemyComp.currentTile, hexToGo);
            NotifiedHexPath.Reverse();

            if (NotifiedHexPath.Count != 0) { NotifiedHexPath.RemoveAt(0); } // remove the first tile since it is already on the path
            
            //ai.NotifiedPush.Fire();
        }
    }
}