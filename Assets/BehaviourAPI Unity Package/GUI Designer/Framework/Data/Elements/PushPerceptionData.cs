
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;

    /// <summary>
    /// Clas that serializes a push perception
    /// </summary>
    [Serializable]
    public class PushPerceptionData
    {
        /// <summary>
        /// The name of the push perception.
        /// </summary>
        public string name;

        /// <summary>
        /// The push perception stored.
        /// </summary>
        [HideInInspector] public PushPerception pushPerception;

        /// <summary>
        /// The id of the target nodes in the system data.
        /// </summary>
        [HideInInspector] public List<string> targetNodeIds = new List<string>();

        /// <summary>
        /// Default construction
        /// </summary>
        public PushPerceptionData()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// Set the <see cref="pushPerception"/> target nodes searching the nodes in system data by id.
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        public void Build(SystemData data)
        {
            pushPerception = new PushPerception();

            if (targetNodeIds.Count > 0)
            {
                var allNodes = data.graphs.SelectMany(g => g.nodes).ToList();
                for (int i = 0; i < targetNodeIds.Count; i++)
                {
                    var node = allNodes.Find(node => node.id == targetNodeIds[i]);
                    var pushTarget = node?.node as IPushActivable;
                    pushPerception.PushListeners.Add(pushTarget);
                }
            }
        }

        public void Build(BSBuildingInfo buildData)
        {
            pushPerception = new PushPerception();

            if (targetNodeIds.Count > 0)
            {
                for (int i = 0; i < targetNodeIds.Count; i++)
                {
                    var node = buildData.NodeMap[targetNodeIds[i]];
                    if(node is IPushActivable target)
                    {
                        pushPerception.PushListeners.Add(target);
                    }
                    else
                    {
                        Debug.LogWarning("BUILD ERROR: Trying to add a node that don't implements IPushActivable to a push perception target list");
                    }
                }
            }
        }
    }
}
