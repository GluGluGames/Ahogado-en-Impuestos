using System;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    public class CreateGraphPanel : ToolPanel
    {
        Action<string, Type> m_OnCreategraphCallback;

        public GraphTypeEntry SelectedEntry;

        TextField graphNameField;

        public CreateGraphPanel(Action<string, Type> onCreategraphCallback) : base("creategraphpanel.uxml")
        {
            Button createBtn = this.Q<Button>("cgp-create-btn");
            createBtn.clicked += OnCreateButton;

            graphNameField = this.Q<TextField>("cgp-name-field");
            
            m_OnCreategraphCallback = onCreategraphCallback;

            ScrollView graphScrollView = this.Q<ScrollView>("cgp-graph-sv");

            foreach(var kvp in BehaviourAPISettings.instance.Metadata.GraphAdapterMap)
            {
                var entry = new GraphTypeEntry(kvp.Key);
                entry.Selected += ChangeSelectedEntry;
                graphScrollView.Add(entry);
            }
        }

        private void ChangeSelectedEntry(GraphTypeEntry entry)
        {
            if (SelectedEntry != null) SelectedEntry.Unselect();
            SelectedEntry = entry;
            if (SelectedEntry != null) SelectedEntry.Select();
        }

        private void OnCreateButton()
        {
            if (SelectedEntry == null) return;

            Type selectedType = SelectedEntry.type;
            m_OnCreategraphCallback?.Invoke(graphNameField.value, selectedType);
            this.Disable();
        }
    }
}

