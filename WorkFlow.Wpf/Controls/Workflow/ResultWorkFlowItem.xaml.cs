using System.ComponentModel;
using System.Threading.Tasks;
using Workflow.Common.Interface;
using Workflow.Common.Enums;
using System.Windows;
using WorkFlow.Wpf.Impl;
using WorkFlow.Controls.Workflow;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Wpf.Controls.Workflow 
{
    public sealed partial class ResultWorkFlowItem : ExecutableNodeBase, IWorkFlowItem, INotifyPropertyChanged, IExecutableNode
    {
        FrameworkElement parent;
        public ResultWorkFlowItem(FrameworkElement parent):base(parent)
        {
            this.InitializeComponent();
            base.Element = this;
            this.DataContext = this;
            this.parent = parent;
            AddConnector(new ConnectorControl { Type = ConnectorType.In, Label = "Input", Height = 25, Width = 25, WorkFlowItem = this });
            OnExecuteAction = async input => {

                MessageBox.Show(input?.ToString()??"Null output");
                return null;
            };
        }

        public async Task Start(params object[] args)
        {
            await Run("Sample");
        }
    }
}
