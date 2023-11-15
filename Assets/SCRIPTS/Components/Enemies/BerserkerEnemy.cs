using GGG.Components.Buildings;
using System.Collections;
using System.Net.Http.Headers;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class MiniEnemy : MonoBehaviour
    {
        public FieldOfView fov;
        public BerserkerEnemyAI ai;
        public EnemyComponent enemyComp;
        [SerializeField] private int maxPatience = 2;
        private int currentPatience = 0;
        private Transform targetTransform = null;

        private void Awake()
        {
            fov.onGainDetection += (trans) =>
            {
                if (targetTransform == null)
                {
                    targetTransform = trans;
                }
                else if (!ai.berserkerMode)
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
                        }
                    }
                }
                else if (trans.gameObject.layer == 14)
                {
                    if (trans.GetInstanceID() != targetTransform.GetInstanceID())
                    {
                        float newDist = Vector3.Distance(transform.position, trans.position);
                        float preDist = Vector3.Distance(transform.position, targetTransform.position);

                        if (newDist < preDist)
                        {
                            targetTransform = trans;
                        }
                    }
                    else
                    {
                        targetTransform = trans;
                    }
                    if(targetTransform.tag == "Player")
                    {
                        enemyComp.movementController.imChasingPlayer = true;
                    }
                    else
                    {
                        enemyComp.movementController.targetEnemy = targetTransform.GetComponent<Enemy>();
                        enemyComp.movementController.imChasingPlayer = false;
                    }
                }
                Debug.Log(targetTransform);
                Debug.Log(targetTransform.GetComponent<Enemy>());
            };

            fov.onLostDetection += () =>
            {
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.lostPlayerPush.Fire();
                //ai.enemyKilledPush.Fire();
                ai.berserkerMode = false;
            };

            ai.StartPatrol += () =>
            {
                Debug.Log("comienzo patrulla");
                enemyComp.movementController.imChasing = false;
                fov.imBlinded = false;
            };

            ai.UpdatePatrol += () =>
            {
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                Debug.Log("comienzo perseguir");
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += countPatience;
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(new Vector3(PlayerPosition.CurrentTile.transform.position.x, transform.position.y, PlayerPosition.CurrentTile.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.SleepMethod += () =>
            {
                Debug.Log("comienzo dormir");
                enemyComp.movementController.onMove -= countPatience;
                currentPatience = 0;
                enemyComp.movementController.movingAllowed = false;
                enemyComp.movementController.imBerserker = false;
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
            };
            
            ai.StartBerserker += () =>
            {
                Debug.Log("Comienzo berserker");
                enemyComp.movementController.imChasing = true;
                enemyComp.movementController.onMove -= countPatience;
                enemyComp.movementController.imBerserker = true;
                ai.berserkerMode = true;
            };

            ai.UpdateBerserker += () =>
            {
                enemyComp.movementController.imBerserker = true;
                ai.berserkerMode = true;
                transform.LookAt(new Vector3(targetTransform.transform.position.x, transform.position.y, targetTransform.transform.position.z));
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                ai.enemyKilledPush.Fire();
                ai.lostPlayerPush.Fire();
                Debug.Log("colisiono con player");
            } else if(other.GetComponent<Enemy>() != null && ai.berserkerMode == true)
            {
                Debug.Log("colisiono con " + other.transform.name);
                Destroy(other.gameObject);
                ai.enemyKilledPush.Fire();
            }
        }

        private void countPatience()
        {
            currentPatience++;
            if (currentPatience == maxPatience)
            {
                ai.lostPatiencePush.Fire();
                currentPatience = 0;
            }
        }

        private IEnumerator OnSleepCoroutine()
        {
            yield return new WaitForSeconds(enemyComp.restTime);
            enemyComp.movementController.movingAllowed = true;
            ai.restedPush.Fire();
        }

    }
}