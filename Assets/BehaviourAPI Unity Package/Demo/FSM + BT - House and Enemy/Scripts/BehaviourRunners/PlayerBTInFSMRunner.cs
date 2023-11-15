using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class PlayerBTInFSMRunner : BehaviourRunner
    {
        [SerializeField] AudioClip keyFoundClip;

        Transform _enemyTransform;
        Transform _door;
        AudioSource _audioSource;

        Vector3 _doorPos;
        Vector3 _keyPos;

        bool _hasKey;

        BSRuntimeDebugger _debugger;

        protected override void Init()
        {
            _enemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
            _door = GameObject.FindGameObjectWithTag("Door").transform;
            _audioSource = GetComponent<AudioSource>();
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }

        protected override BehaviourGraph CreateGraph()
        {
            var fsm = new StateMachines.FSM();

            var enemyNearPerception = new DistancePerception(_enemyTransform, 10f);
            _doorPos = new Vector3(_door.position.x, transform.position.y, _door.position.z);

            var bt = CreateFindKeySubBT();

            var doorState = fsm.CreateState("Go to home", new WalkAction(_doorPos)); // Caminar hacia la casa
            var keyState = fsm.CreateState("Search key", new SubsystemAction(bt));      // Subarbol de buscar la llave
            var houseState = fsm.CreateState("Enter the house", new FunctionalAction(EnterTheHouse));                        // Destroy
            var runState = fsm.CreateState("Runaway", new FleeAction(_enemyTransform, 8f, 10f, 3f));

            // Cuando llega a la puerta comprueba si tiene la llave
            fsm.CreateTransition("Success to enter", doorState, keyState, statusFlags: StatusFlags.Finished);

            // Cuando tiene la llave y está en la puerta, entra en la casa
            fsm.CreateTransition("Fail to enter", keyState, houseState, statusFlags: StatusFlags.Finished);

            // Las acciones se interrumpen si el enemigo está cerca.
            fsm.CreateTransition("Interrupt key", keyState, runState, enemyNearPerception);
            fsm.CreateTransition("Interrupt going to door", doorState, runState, enemyNearPerception);

            // Cuando consigue huir vuelve hacia la puerta
            fsm.CreateTransition("Return to door", runState, doorState, statusFlags: StatusFlags.Finished);

            _debugger.RegisterGraph(fsm, "main");
            _debugger.RegisterGraph(bt, "key subtree");
            return fsm;
        }

        private BehaviourTree CreateFindKeySubBT()
        {
            var bt = new BehaviourTree();

            var keyPos = GameObject.FindGameObjectWithTag("Key").transform.position;
            var l1 = bt.CreateLeafNode(new WalkAction(keyPos));
            var hasKey = bt.CreateDecorator<ConditionNode>(l1).SetPerception(new ConditionPerception(() => !_hasKey));
            var succeder = bt.CreateDecorator<SuccederNode>(hasKey);

            var l2 = bt.CreateLeafNode("Return to door", new WalkAction(_doorPos));
            var seqRoot = bt.CreateComposite<SequencerNode>(false, succeder, l2);
            bt.SetRootNode(seqRoot);
            return bt;
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