using Windows.UI.Xaml;
using System.ComponentModel;
using System.Threading.Tasks;
using WorkFlow.Impl;
using Workflow.Common.Interface;
using Workflow.Common.Enums;
using Workflow.Common.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class ActionWorkFlowItem : ExecutableNodeBase, IWorkFlowItem, INotifyPropertyChanged, IExecutableNode
    {
        FrameworkElement parent;
        public ActionWorkFlowItem(FrameworkElement parent):base(parent)
        {
            this.InitializeComponent();
            base.Element = this;
            this.DataContext = this;
            this.parent = parent;
            AddConnector(new ConnectorControl { Type = ConnectorType.In, Label = "Input", Height = 25, Width = 25, WorkFlowItem = this });
            AddConnector(new ConnectorControl { Type = ConnectorType.Out, Label = "Output", Height = 25, Width = 25, WorkFlowItem = this });
            OnExecuteAction = async input => {
                await Task.Delay(5000);
                return input;
            };
        }

       
    }
}
