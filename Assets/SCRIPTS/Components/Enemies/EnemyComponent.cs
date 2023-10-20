using GGG.Components.Buildings;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    [SerializeField] private Enemy Enemy;

    public Action<Action, EnemyComponent> OnEnemyInteract;


}
