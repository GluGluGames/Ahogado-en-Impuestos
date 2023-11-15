using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class PlayerBTInFSMEditorRunner : EditorBehaviourRunner
    {
        [SerializeField] AudioClip keyFoundClip;

        Transform _enemyTransform;
        Transform _door;
        AudioSource _audioSource;

        Vector3 _doorPos;
        Vector3 _keyPos;

        bool _hasKey;

        protected override void Init()
        {
            _enemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
            _door = GameObject.FindGameObjectWithTag("Door").transform;
            _doorPos = new Vector3(_door.position.x, transform.position.y, _door.position.z);
            _keyPos = GameObject.FindGameObjectWithTag("Key").transform.position;
            _audioSource = GetComponent<AudioSource>();
            base.Init();
        }

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            var mainGraph = graphMap["main"];
            var subgraph = graphMap["key subtree"];

            mainGraph.FindNode<State>("go to home").Action = new WalkAction(_doorPos);
            mainGraph.FindNode<State>("enter house").Action = new FunctionalAction(EnterTheHouse);
            mainGraph.FindNode<State>("run").Action = new FleeAction(_enemyTransform, 1.5f, 10, 3);

            Perception enemyPerception = new DistancePerception(_enemyTransform, 10);
            mainGraph.FindNode<StateTransition>("house to running").Perception = enemyPerception;
            mainGraph.FindNode<StateTransition>("key to running").Perception = enemyPerception;

            subgraph.FindNode<ConditionNode>("has no key").SetPerception(new ConditionPerception(() => !_hasKey));
            subgraph.FindNode<LeafNode>("walk to key").Action = new WalkAction(_keyPos);
            subgraph.FindNode<LeafNode>("return to door").Action = new WalkAction(_doorPos);
        }

        private void EnterTheHouse()
        {
            Destroy(gameObject, 2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Key")
            {
                _audioSource.clip = keyFoundClip;
                _audioSource.Play();
                _hasKey = true;
                other.gameObject.SetActive(false);
            }
        }

    }

}