using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BoyBTRunner : BehaviourRunner
    {
        Door _door;
        [SerializeField] AudioClip doorOpenClip;
        [SerializeField] AudioClip keyFoundClip;
        [SerializeField] AudioClip explosionClip;

        [Header("VFX")]
        [SerializeField] GameObject explosionFX;

        private bool _hasKey, _keyFound;
        AudioSource _audioSource;
        NavMeshAgent meshAgent;

        BSRuntimeDebugger _debugger;

        protected override void Init()
        {
            _door = FindObjectOfType<Door>();
            _audioSource = GetComponent<AudioSource>();
            meshAgent = GetComponent<NavMeshAgent>();
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }

        protected override BehaviourGraph CreateGraph()
        {
            var bt = new BehaviourTree();
            var doorPos = new Vector3(_door.transform.position.x, transform.position.y, _door.transform.position.z);

            var walkToDoorAction = new WalkAction(doorPos);
            var openDoorAction = new FunctionalAction(OpenDoor, DoorStatus);
            var explodeAction = new FunctionalAction(SmashDoor, () => Status.Success);
            var enterAction = new FunctionalAction(EnterTheHouse, () => Status.Success);
            var findKeyAction = new FunctionalAction(FindKey, IsKeyObtained, KeyFound);

            var walkToDoor = bt.CreateLeafNode("walkToDoor", walkToDoorAction); // Camina a la puerta

            var openDoor = bt.CreateLeafNode("open door", openDoorAction); // Abre la puerta


            var findKey = bt.CreateLeafNode("find key", findKeyAction);
            var returnToDoor = bt.CreateLeafNode("use key", walkToDoorAction); // Camina a la puerta
            var useKey = bt.CreateLeafNode("try unlock", openDoorAction); // Abre la puerta

            var explode = bt.CreateLeafNode("explode", explodeAction); // Hace explotar la puerta

            var enter = bt.CreateLeafNode("enter", enterAction); // Entra a la casa

            var seq = bt.CreateComposite<SequencerNode>("key seq", false, findKey, returnToDoor, useKey);
            var sel = bt.CreateComposite<SelectorNode>("sel", false, openDoor, seq, explode);
            var root = bt.CreateComposite<SequencerNode>("root", false, walkToDoor, sel, enter);

            bt.SetRootNode(root);
            _debugger.RegisterGraph(bt, "main");
            return bt;
        }

        private void SmashDoor()
        {
            GameObject explosion = Instantiate(explosionFX, _door.transform);
            _audioSource.clip = explosionClip;
            _audioSource.Play();
            Destroy(explosion, 3);
        }

        private Status DoorStatus()
        {
            return !_door.IsClosed ? Status.Success : Status.Failure;
        }

        private void OpenDoor()
        {
            if (!_door.IsClosed)
            {
                _audioSource.clip = doorOpenClip;
                _audioSource.Play();
            }
        }

        private void EnterTheHouse()
        {
            Destroy(gameObject, 2);
        }

        private void FindKey()
        {
            GameObject key = GameObject.FindGameObjectWithTag("Key");

            if (key != null)
            {
                _keyFound = true;
                meshAgent.isStopped = false;
                meshAgent.destination = new Vector3(key.transform.position.x, transform.position.y, key.transform.position.z);
            }
        }

        private Status IsKeyObtained()
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

        private void KeyFound()
        {
            meshAgent.isStopped = true;
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