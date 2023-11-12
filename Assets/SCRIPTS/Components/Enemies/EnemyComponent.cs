using UnityEngine;

namespace GGG.Components.Enemies
{
    public class EnemyComponent : Enemy
    {
        private IMovementController MovementController;
        public FieldOfView fov;
        public NormalEnemyAI ai;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            MovementController = new RandomMovement();
            MovementController.SetGameObject(gameObject);
            MovementController.SetAlwaysVisible(_alwaysVisible);
            if (isDirty) MovementController.SetCurrentTile(currentTile);

            fov.onGainDetection += () =>
            {
                currentTile = MovementController.GetCurrentTile();
                ai.playerDetected.Fire();
            };

            fov.onLostDetection += () =>
            {
                //ai.playerLost.Fire();
                //MovementController.LaunchOnDisable();
            };

            ai.StartPatrol += () =>
            {
                MovementController.LaunchOnStart();
            };

            ai.UpdatePatrol += () =>
            {
                if (currentTile != null) MovementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartChase += () =>
            {
                Debug.Log("te persigo");
                MovementController = new ChasingMovement();
                MovementController.SetGameObject(gameObject);
                MovementController.SetAlwaysVisible(_alwaysVisible);
                MovementController.SetCurrentTile(currentTile);
                MovementController.LaunchOnStart();
            };

            ai.UpdateChase += () =>
            {
                if (currentTile != null) MovementController.LaunchOnUpdate();
                return BehaviourAPI.Core.Status.Running;
            };

            ai.StartSleep += () => { };
        }

        private void Start()
        {
            //MovementController.LaunchOnStart();
        }

        private void Update()
        {
            if (isDirty)
            {
                MovementController.SetCurrentTile(currentTile);
                isDirty = false;
            }
            //if (currentTile != null) MovementController.LaunchOnUpdate();
        }

        private void OnDisable()
        {
            MovementController.LaunchOnDisable();
        }
    }
}