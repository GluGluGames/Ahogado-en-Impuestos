using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BadBoyRunner : BehaviourRunner
    {
        public Transform[] routePoints;

        BSRuntimeDebugger _debugger;

        protected override void Init()
        {
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }

        protected override BehaviourGraph CreateGraph()
        {
            var patrol = new PathingAction(routePoints.Select(tf => tf.position).ToList(), .1f);

            var bt = new BehaviourTree();
            var leaf = bt.CreateLeafNode(patrol);
            var root = bt.CreateDecorator<LoopNode>(leaf);
            bt.SetRootNode(root);
            _debugger.RegisterGraph(bt, "main");
            return bt;
        }
    }

}