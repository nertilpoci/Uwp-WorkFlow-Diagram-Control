using System.Threading.Tasks;
using System.Windows;
using Workflow.Common.Enums;
using Workflow.Common.Implementation;
using Workflow.Common.ViewModels;
using WorkFlow.Wpf.Impl;
using WorkFlow.Wpf.WorkFlowItems.Controls;

namespace WorkFlow.Wpf.WorkFlowItems.Items
{
    public class ActionWorkFlowItem: WpfExecutableNodeBase
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

            OnExecuteAction = async input => { await Task.Delay(5000); return input; };
        }
    }
}
