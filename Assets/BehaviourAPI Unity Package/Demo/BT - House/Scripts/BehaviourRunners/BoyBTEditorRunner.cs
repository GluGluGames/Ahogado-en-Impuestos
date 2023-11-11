using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BoyBTEditorRunner : EditorBehaviourRunner
    {
        [SerializeField] AudioClip doorOpenClip;
        [SerializeField] AudioClip keyFoundClip;
        [SerializeField] AudioClip explosionClip;
        [SerializeField] GameObject explosionFX;
        Door _door;
        AudioSource _audioSource;

        private bool _hasKey, _keyFound;
        NavMeshAgent _meshAgent;

        protected override void Init()
        {
            _door = FindObjectOfType<Door>();
            _audioSource = GetComponent<AudioSource>();
            _meshAgent = GetComponent<NavMeshAgent>();
            base.Init();
        }

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            var doorPos = new Vector3(_door.transform.position.x, transform.position.y, _door.transform.position.z);
            BehaviourGraph mainGraph = graphMap["main"];
            mainGraph.FindNode<LeafNode>("go to door").Action = new WalkAction(doorPos);
            mainGraph.FindNode<LeafNode>("return to door").Action = new WalkAction(doorPos);
        }

        public void SmashDoor()
        {
            GameObject explosion = Instantiate(explosionFX, _door.transform);
            _audioSource.clip = explosionClip;
            _audioSource.Play();
            Destroy(explosion, 3);
        }

        public Status EndWithSuccess() => Status.Success;

        public Status DoorStatus()
        {
            return !_door.IsClosed ? Status.Success : Status.Failure;
        }

        public void OpenDoor()
        {
            if (!_door.IsClosed)
            {
                _audioSource.clip = doorOpenClip;
                _audioSource.Play();
            }
        }

        public void EnterTheHouse()
        {
            Destroy(gameObject, 2);
        }

        public void FindKey()
        {
            GameObject key = GameObject.FindGameObjectWithTag("Key");

            if (key != null)
            {
                _keyFound = true;
                _meshAgent.isStopped = false;
                _meshAgent.destination = new Vector3(key.transform.position.x, transform.position.y, key.transform.position.z);
            }
        }

        public Status IsKeyObtained()
        {
            if (_hasKey)
            {
                return Status.Success;
            }
            else if (!_keyFound)
            {
                return Status.Failure;
            }
            else
            {
                return Status.Running;
            }
        }

        public void KeyFound()
        {
            _meshAgent.isStopped = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Key")
            {
                _audioSource.clip = keyFoundClip;
                _audioSource.Play();
                _hasKey = true;
                _door.IsClosed = false;

                other.gameObject.SetActive(false);
            }
        }
    }

}