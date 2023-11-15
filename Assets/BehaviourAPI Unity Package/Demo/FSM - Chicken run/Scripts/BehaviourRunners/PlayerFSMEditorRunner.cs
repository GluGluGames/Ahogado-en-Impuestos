using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class PlayerFSMEditorRunner : EditorBehaviourRunner
    {
        [SerializeField] private float minDistanceToChicken = 5;
        [SerializeField] private Transform chicken;
        [SerializeField] private Transform origin;

        private PushPerception _click;

        // Update is called once per frame
        protected override void OnUpdated()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _click.Fire();
            }
            base.OnUpdated();
        }

        public bool CheckDistanceToChicken()
        {
            return Vector3.Distance(transform.position, chicken.transform.position) < minDistanceToChicken;
        }

        public void Restart()
        {
            transform.position = origin.position;
        }

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            _click = pushPerceptionMap["click"];
        }
    }
}