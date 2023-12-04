using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;
    public abstract class ElementSearchWindowProvider<T> : ScriptableObject, ISearchWindowProvider where T : class
    {
        public SystemData Data;

        private Action<T> m_Callback;
        private Func<T, bool> m_Filter;

        public abstract List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context);

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            m_Callback?.Invoke(SearchTreeEntry.userData as T);
            return true;
        }

        public static E Create<E>(Action<T> callback, Func<T, bool> filter = null) where E : ElementSearchWindowProvider<T>
        {
            E window = CreateInstance<E>();
            window.m_Callback = callback;
            window.m_Filter = filter;
            window.OnCreate();
            return window;
        }

        protected abstract void OnCreate();

        protected bool IsValidElement(T element)
        {
            return m_Filter?.Invoke(element) ?? true;
        }

        public void Open()
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),  this);
        }
    }

    public class GraphSearchWindowProvider : ElementSearchWindowProvider<GraphData>
    {
        public static GraphSearchWindowProvider Instance;

        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var list = new List<SearchTreeEntry>();

            list.AddGroup("Graphs", 0);

            var graphList = Data.graphs;
            for (int i = 0; i < graphList.Count; i++)
            {
                if (IsValidElement(graphList[i]))
                {
                    list.AddEntry($"{i + 1} - {graphList[i].name}", 1, graphList[i]);
                }
            }
            return list;
        }

        protected override void OnCreate()
        {
            Instance = this;
        }
    }

    public class NodeSearchWindowProvider : ElementSearchWindowProvider<NodeData>
    {
        public static NodeSearchWindowProvider Instance;

        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var list = new List<SearchTreeEntry>();

            list.AddGroup("Nodes", 0);

            var graphList = Data.graphs;

            for (int i = 0; i < graphList.Count; i++)
            {
                list.AddGroup($"{i + 1} - {graphList[i].name}", 1);
                for(int j = 0; j < graphList[i].nodes.Count; j++)
                {
                    if(IsValidElement(graphList[i].nodes[j]))
                    {
                        var nodeData = graphList[i].nodes[j];
                        list.AddEntry($"{nodeData.name} ({nodeData.node.GetType().Name})", 2, nodeData);
                    }
                }
            }
            return list;
        }

        protected override void OnCreate()
        {
            Instance = this;
        }
    }
}
