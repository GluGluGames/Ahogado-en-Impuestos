using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BadBoyEditorRunner : EditorBehaviourRunner
    {
        public Transform[] routePoints;

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            graphMap["main"].FindNode<LeafNode>("patrol").Action = new PathingAction(routePoints.Select(tf => tf.position).ToList(), .1f);

        }
    }

}