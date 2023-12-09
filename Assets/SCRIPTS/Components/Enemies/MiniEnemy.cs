using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections;
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

        private void Awake()
        {
            fov.onGainDetection += (trans) =>
            {
                if (trans.tag == "Player")
                {
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    ai.playerDetectedPush.Fire();
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
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.lostPatiencePush.Fire();
            };

            ai.StartPatrol += () =>
            {
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
                StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += countTiredness;
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
                enemyComp.movementController.onMove -= countTiredness;
                enemyComp.movementController.movingAllowed = false;
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StateUI.ChangeState(StateIcon.SleepState);
                StartCoroutine(OnSleepCoroutine());
            };

            ai.FleeMethod += () =>
            {
                enemyComp.movementController.onMove -= countTiredness;
                StateUI.ChangeState(StateIcon.FleeState);
                HexTile destination = enemyComp.movementController.Flee(3);
                StartCoroutine(checkFleeingStatus(destination));
            };
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
            yield return new WaitForSeconds(enemyComp.restTime);
            enemyComp.movementController.movingAllowed = true;
            ai.restedPush.Fire();
        }

        private IEnumerator checkFleeingStatus(HexTile destinationCheck)
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

        private IEnumerator onStaminaRecharge()
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
    }
}