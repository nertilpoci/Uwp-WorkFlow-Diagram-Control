using System.Windows;
using System.Windows.Controls;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using WorkFlow.Wpf.Extensions;

namespace WorkFlow.Wpf.WorkFlowItems.Controls
{
    public sealed partial class WorkFlowItemContentBase : UserControl, IWorkFlowItemContent
    {
        public WorkFlowItemContentBase()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public IWorkFlowItemContentContext ItemContentContext { get; set; }

        public void AddConnector(IConnector connector)
        {
            if (connector.Type == ConnectorType.In) inputConnectors.Children.Add(connector.UIElement.GetUiElement<FrameworkElement>());
            else outputConnectors.Children.Add(connector.UIElement.GetUiElement<FrameworkElement>());
            
            
        }
    }
}
