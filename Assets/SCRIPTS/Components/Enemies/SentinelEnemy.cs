using GGG.Components.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                //StateUI.ChangeState(StateIcon.PatrolState);
            };

            ai.UpdatePatrol += () =>
            {
                if (fov.FieldOfViewCheck().Count == 0) return BehaviourAPI.Core.Status.Running;

                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                Debug.Log("VEO PLAYER");
                EnemiesToNotify();

                return BehaviourAPI.Core.Status.Success;
            };

            ai.StartNotify += () =>
            {
                Debug.Log("Notifico");
            };

            ai.UpdateNotify += () =>
            {
                EnemiesToNotify();
                return BehaviourAPI.Core.Status.Success;
            };

            ai.StartSleep += () =>
            {
                Debug.Log("duermo");
                //StateUI.ChangeState(StateIcon.SleepState);
            };

            ai.UpdateSleep += () =>
            {
                if(CurrentTimeSleeping >= enemyComp.restTime) 
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

            foreach (Transform t in EnemiesOnRange)
            {
                if (t.TryGetComponent(out NormalEnemy normalEnemy))
                {
                    normalEnemy.GetNotified(enemyComp.movementController.GetCurrentTile());
                }
                else
                {
                    t.TryGetComponent(out MiniEnemy miniEnemy);
                    miniEnemy.GetNotified(enemyComp.movementController.GetCurrentTile());
                }
            }
        }
    }
}