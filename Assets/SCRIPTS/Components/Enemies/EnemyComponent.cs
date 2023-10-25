using System;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class EnemyComponent : Enemy
    {
        private IMovementController MovementController;
        
        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            MovementController = new RandomMovement();
            MovementController.SetGameObject(gameObject);
            MovementController.SetAlwaysVisible(_alwaysVisible);
            if (isDirty) MovementController.SetCurrentTile(currentTile);
        }

        void Start()
        {
            MovementController.LaunchOnStart();
        }

        void Update()
        {
            if (isDirty)
            {
                MovementController.SetCurrentTile(currentTile);
                isDirty = false;
            }
            if (currentTile != null) MovementController.LaunchOnUpdate();
        }

        private void OnDisable()
        {
            MovementController.LaunchOnDisable();
        }
    }
}