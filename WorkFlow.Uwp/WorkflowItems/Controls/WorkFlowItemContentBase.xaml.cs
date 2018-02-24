using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.WorkFlowItems.Controls
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
            if (connector.Type == ConnectorType.In) inputConnectors.Children.Add(connector.UIElement.ToFrameworkElement());
            else outputConnectors.Children.Add(connector.UIElement.GetUiElement<FrameworkElement>());
            
            
        }
    }
}
