using System.Threading.Tasks;
using Windows.UI.Xaml;
using Workflow.Common.Enums;
using Workflow.Common.ViewModels;
using WorkFlow.Impl;
using WorkFlow.WorkFlowItems.Controls;

namespace WorkFlow.WorkFlowItems.Items
{
    public class ActionWorkFlowItem: UWPExecutableNodeBase
    {
        FrameworkElement _parent;
        public ActionWorkFlowItem(FrameworkElement parent) : base(parent)
        {
            _parent = parent;
            this.ItemContent = new WorkFlowItemContentBase();
            this.ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();

            this.UIElement = new WorkFlowItemControl(_parent) { DataContext = this };

            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.In, Label = "Input", WorkFlowItem = this });
            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.Out, Label = "Output", WorkFlowItem = this });

            OnExecuteAction = async input => {
                await Task.Delay(5000); return input; };
        }
    }
}
