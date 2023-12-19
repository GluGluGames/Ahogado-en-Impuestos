using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class SentinelEnemy : MonoBehaviour
    {
        public FieldOfViewUpgraded fov;
        public FieldOfViewUpgraded enemiesOnRangeFOV;
        public SentinelEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private EnemyStateUI StateUI;

        private List<Transform> EnemiesOnRange;

        private float CurrentTimeSleeping = 0f;

        private void Awake()
        {
            ai.StartPatrol += () =>
            {
                Debug.Log("Patrullo");
                fov.imBlinded = false;
                StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (fov.FieldOfViewCheck().Count == 0) return BehaviourAPI.Core.Status.Running;

                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                EnemiesToNotify();

                return BehaviourAPI.Core.Status.Success;
            };

            ai.StartNotify += () =>
            {

            };

            ai.UpdateNotify += () =>
            {
                EnemiesToNotify();
                return BehaviourAPI.Core.Status.Success;
            };

            ai.StartSleep += () =>
            {
                StateUI.ChangeState(StateIcon.WarningState);
            };

            ai.UpdateSleep += () =>
            {
                if (CurrentTimeSleeping >= enemyComp.restTime)
                {
                    CurrentTimeSleeping = 0f;
                    return BehaviourAPI.Core.Status.Success;
                }

                CurrentTimeSleeping += Time.deltaTime;

                return BehaviourAPI.Core.Status.Running;
            };
        }

        private void EnemiesToNotify()
        {
            EnemiesOnRange = enemiesOnRangeFOV.FieldOfViewCheck();
            HexTile hexTileNotified = enemyComp.movementController.ChooseNeighbourTileAway((int)fov.radius, 0, enemyComp.movementController.GetCurrentTile());
            foreach (Transform t in EnemiesOnRange)
            {
                if (t.TryGetComponent(out NormalEnemy normalEnemy))
                {
                    normalEnemy.GetNotified(hexTileNotified);
                }
                else
                {
                    t.TryGetComponent(out MiniEnemy miniEnemy);
                    miniEnemy.GetNotified(hexTileNotified);
                }
            }
        }
    }
}