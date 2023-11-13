using GGG.Components.Buildings;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class NormalEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public NormalEnemyAI ai;
        public EnemyComponent enemyComp;

        private void Awake()
        {
            fov.onGainDetection += () =>
            {
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.playerDetected.Fire();
            };

            fov.onLostDetection += () =>
            {
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.playerLost.Fire();
            };

            ai.StartPatrol += () =>
            {
                Debug.Log("patrullo");
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
            };

            ai.UpdatePatrol += () =>
            {
                transform.LookAt(PlayerPosition.PlayerPos);
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                Debug.Log("te persigo");
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
            };

            ai.UpdateChase += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartSleep += () =>
            {
                enemyComp.movementController.movingAllowed = false;
                Debug.Log("Me duermo");
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine()); 
            };

      
        }

        private void Start()
        {
        }

        private void Update()
        {

        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(2.0f);
            enemyComp.movementController.movingAllowed = true;
            ai.RestedPush.Fire();
        }


    }
}