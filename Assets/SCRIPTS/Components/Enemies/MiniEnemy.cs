using System;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class MiniEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public MiniEnemyAI ai;
        public EnemyComponent enemyComp;

        private void Awake()
        {
            fov.onGainDetection += (trans) =>
            {
                if (trans.tag == "Player")
                {
                    Debug.Log("detecto player");
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    ai.playerDetectedPush.Fire();
                }
                else if(trans.GetComponent<EnemyComponent>() != null)
                {
                    Debug.Log("detecto enemigo");
                    EnemyComponent aux = trans.GetComponent<EnemyComponent>();
                    if(aux.size > enemyComp.size) 
                    {
                        enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                        ai.detectBiggerEnemyPush.Fire();
                    }
                }

            };

            fov.onLostDetection += () =>
            {
                Debug.Log("pierdo deteccion");
                //enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                //ai.lostPatiencePush.Fire();
            };

            /*
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

            ai.SleepMethod += () =>
            {
                enemyComp.movementController.movingAllowed = false;
                Debug.Log("Me duermo");
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
            };
            */
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
            ai.restedPush.Fire();
        }
    }
}