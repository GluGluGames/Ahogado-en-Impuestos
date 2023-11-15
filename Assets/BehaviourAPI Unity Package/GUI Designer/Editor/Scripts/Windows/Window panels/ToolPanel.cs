using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    public class ToolPanel : VisualElement
    {
        bool canClose = true;

        public ToolPanel(string relativePath)
        {
            VisualTreeAsset asset = BehaviourAPISettings.instance.GetLayoutAsset("Panels/" + relativePath);
            asset.CloneTree(this);
            this.StretchToParentSize();

            Button closeBtn = this.Q<Button>("cgp-close-btn");
            closeBtn.clicked += ClosePanel;
        }

        public void ClosePanel()
        {
            if (canClose)
            {
                this.Disable();
            }
        }

        public virtual void Open(bool canClose = true)
        {
            this.canClose = canClose;
            this.Enable();
        }
    }
}
