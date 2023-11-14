using GGG.Components.Buildings;
using System.Collections;
using System.Transactions;
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
        private int tiredness = 0;

        private void Awake()
        {
            fov.onGainDetection += (trans) =>
            {
                if (trans.tag == "Player")
                {
                    //Debug.Log("detecto player");
                    enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                    ai.playerDetectedPush.Fire();
                }
                else if (trans.GetComponent<EnemyComponent>() != null)
                {
                    //Debug.Log("detecto enemigo");
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
                Debug.Log("pierdo deteccion");
                enemyComp.currentTile = enemyComp.movementController.GetCurrentTile();
                ai.lostPatiencePush.Fire();
            };

            ai.StartPatrol += () =>
            {
                Debug.Log("patrullo");
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
                Debug.Log("te persigo");
                enemyComp.movementController.currentPath.Clear();
                enemyComp.movementController.imChasing = true;
                fov.imBlinded = false;
                enemyComp.movementController.onMove += countTiredness;
            };

            ai.UpdateChase += () =>
            {
                transform.LookAt(PlayerPosition.PlayerPos);
                if (enemyComp.currentTile != null) enemyComp.movementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.SleepMethod += () =>
            {
                Debug.Log("Me duermo");
                enemyComp.movementController.onMove -= countTiredness;
                enemyComp.movementController.movingAllowed = false;
                fov.canSeePlayer = false;
                fov.imBlinded = true;
                StartCoroutine(OnSleepCoroutine());
            };

            ai.FleeMethod += () =>
            {
                Debug.Log("huyo");
                enemyComp.movementController.onMove -= countTiredness;
                HexTile destination = enemyComp.movementController.Flee(3);
                StartCoroutine(checkFleeingStatus(destination));
            };


        }

        private void Start()
        {
        }

        private void Update()
        {
        }


        private void countTiredness()
        {
            enemyComp.movementController.onMove += () =>
            {
                tiredness++;
                if (tiredness == stamina)
                {
                    Debug.Log("descanso");
                    StartCoroutine(onStaminaRecharge());
                    tiredness = 0;
                }
            };
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
            fov.imBlinded = true;
            enemyComp.movementController.movingAllowed = false;
            yield return new WaitForSeconds(staminaRechargeTime);
            fov.imBlinded = false;
            enemyComp.movementController.movingAllowed = true;
            Debug.Log("dejo de descansar");
        }
    }
}