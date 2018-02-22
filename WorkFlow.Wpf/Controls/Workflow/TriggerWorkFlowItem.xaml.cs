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
    public sealed partial class TriggerWorkFlowItem : ExecutableNodeBase,ITriggerNode
    {
        FrameworkElement parent;
        public TriggerWorkFlowItem(FrameworkElement parent):base(parent)
        {
            this.InitializeComponent();
            base.Element = this;
            this.DataContext = this;
            this.parent = parent;
            AddConnector(new ConnectorControl { Type = ConnectorType.Out, Label = "Output", Height = 25, Width = 25, WorkFlowItem = this });
            OnExecuteAction = async input => {
             
                return input;
            };
            this.MouseRightButtonDown += (s, e) => { e.Handled = true; };
        }

        public async Task Start(params object[] args)
        {
            await Run("Sample");
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            await Start();
        }
    }
}
