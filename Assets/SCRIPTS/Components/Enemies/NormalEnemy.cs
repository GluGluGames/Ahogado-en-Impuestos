using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class NormalEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public NormalEnemyAI ai;
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
                    ai.playerDetected.Fire();
                }
            };

            fov.onLostDetection += () =>
            {
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.playerLost.Fire();
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
                StateUI.ChangeState(StateIcon.ChasingState);
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += CountTiredness;
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
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
                StartCoroutine(onStaminaRecharge());
                tiredness = 0;
            }
        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(2.0f);
            enemyComp.movementController.movingAllowed = true;
            ai.RestedPush.Fire();
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

        public void GetNotified(HexTile hexToGo)
        {
            Debug.Log("soy notificado");
        }
    }
}