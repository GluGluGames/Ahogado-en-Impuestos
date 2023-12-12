using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using System;

    /// <summary>
    /// Structure used to build a runtime behaviour system used serialized data
    /// </summary>
    public class BSBuildingInfo
    {
        /// <summary>
        /// Used to generate the reflected methods.
        /// </summary>
        public Component Runner { get; private set; }

        /// <summary>
        /// The root of the building behaviour systems. Used by asset subsystems to merge itselfs in a single system.
        /// </summary>
        public SystemData RootSystemData { get; private set; }

        /// <summary>
        /// Used to find the behaviour engine references.
        /// </summary>
        public Dictionary<string, BehaviourGraph> GraphMap { get; private set; }

        /// <summary>
        /// Used to find the node references.
        /// </summary>
        public Dictionary<string, Node> NodeMap { get; private set; }

        /// <summary>
        /// The identificator of the current subsystem
        /// </summary>
        public string SubsystemName { get; private set; } 

        /// <summary>
        /// Create a new build data.
        /// </summary>
        /// <param name="component">The component used to find the behaviour engine references.</param>
        /// <param name="systemData">The behaviour system used to create the graph and node maps.</param>
        public BSBuildingInfo(Component component, SystemData systemData)
        {
            Runner = component;
            RootSystemData = systemData;

            GraphMap = systemData.graphs.ToDictionary(g => g.id, g => g.graph);
            NodeMap = systemData.graphs.SelectMany(g => g.nodes).ToDictionary(n => n.id, n => n.node);

            SubsystemName = "";
        }

        /// <summary>
        /// Create a new build data for a nested system
        /// </summary>
        /// <param name="previousData">The previous system data</param>
        /// <param name="systemData"></param>
        public BSBuildingInfo(BSBuildingInfo previousData, SystemData systemData, string subsystemName) 
        { 
            Runner = previousData.Runner;
            RootSystemData = previousData.RootSystemData;

            GraphMap = systemData.graphs.ToDictionary(g => g.id, g => g.graph);
            NodeMap = systemData.graphs.SelectMany(g => g.nodes).ToDictionary(n => n.id, n => n.node);

            SubsystemName = previousData.SubsystemName + subsystemName + ":";
        }
    }
}
