using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    /// <summary>
    /// Generic Edge connector listener class.
    /// </summary>
    /// <typeparam name="TEdge">The type of the edges that t</typeparam>
    public class CustomEdgeConnector<TEdge> : IEdgeConnectorListener where TEdge : Edge
    {
        private Action<TEdge> m_callback = null;
        private Action<TEdge, Vector2> m_callbackOutsidePort = null;

        public CustomEdgeConnector(Action<TEdge> callback, Action<TEdge, Vector2> callbackOutsidePort = null)
        {
            m_callback = callback;
            m_callbackOutsidePort = callbackOutsidePort;
        }

        public void OnDrop(GraphView graphView, Edge edge) => m_callback?.Invoke(edge as TEdge);

        public void OnDropOutsidePort(Edge edge, Vector2 position) => m_callbackOutsidePort?.Invoke(edge as TEdge, position);
    }
}
