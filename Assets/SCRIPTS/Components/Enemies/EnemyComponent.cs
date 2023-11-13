using BehaviourAPI.Core;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class EnemyComponent : Enemy
    {
        [HideInInspector] public RandomMovement movementController;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            movementController = new RandomMovement();
            movementController.SetGameObject(gameObject);
            movementController.SetAlwaysVisible(_alwaysVisible);
            if (isDirty) movementController.SetCurrentTile(currentTile);
        }

        
        private void Start()
        {
            Debug.Log(movementController.currentTile);

            movementController.LaunchOnStart();
        }

        private void Update()
        {
            if (isDirty)
            {
                movementController.SetCurrentTile(currentTile);
                isDirty = false;
            } 

            if(movementController != null)
            {
                //currentTile = movementController.GetCurrentTile();
            }
        }

        private void OnDisable()
        {
            movementController.LaunchOnDisable();
        }
    }
}