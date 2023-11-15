using UnityEngine;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Core;
    using UnityToolkit;

    [System.Serializable]
    public class BSRuntimeEventHandler
    {
        private string k_LogTemplate = @"<color=cyan>[{0}]:</color> Node <color=cyan>{1}</color> changed to <b><color={2}>{3}</color></b>
Frame: {4} | Graph : {5}";

        [Tooltip("Enable to display node status changes in console")]
        public bool debugStatusChanges;

        [Tooltip("The name used to print the console logs")]
        public string identifier;

        public Object Context { get; set; }

        public void RegisterEvents(BehaviourGraph graph, string graphName)
        {
            var dict = graph.GetNodeNames();
            foreach (var node in graph.NodeList)
            {
                if (node is IStatusHandler handler)
                {
                    handler.StatusChanged += (status) => DebugStatusChanged(dict.GetValueOrDefault(node) ?? "unnamed", status, graphName);
                }
            }
        }

        private void DebugStatusChanged(string name, Status status, string graphName)
        {
            if (!debugStatusChanges) return;

            var colorTag = $"#{ColorUtility.ToHtmlStringRGB(status.ToColor())}";
            string id = string.IsNullOrEmpty(identifier) ? "DEBUGGER" : identifier;
            Debug.LogFormat(Context, k_LogTemplate, id, name, colorTag, status, Time.frameCount, graphName);
        }
    }
}
