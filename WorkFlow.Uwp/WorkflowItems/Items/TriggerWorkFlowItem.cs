using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.ViewModels;
using WorkFlow.Impl;
using WorkFlow.WorkFlowItems.Controls;

namespace WorkFlow.WorkFlowItems.Items
{
    public class TriggerWorkFlowItem: UWPExecutableNodeBase, ITriggerNode
    {
        public TriggerWorkFlowItem(FrameworkElement parent) : base(parent)
        {


            this.ItemContent = new WorkFlowItemContentBase();
            this.ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();

            OnExecuteAction = async input => { await Task.Delay(5000); return input;  };
           
            var uiElement = new WorkFlowItemControl(parent) { DataContext = this };
            MenuFlyoutItem mn = new MenuFlyoutItem() { Text = "Trigger" };
            mn.Click += async (s, e) => { await Start(); };
            uiElement.AddContextMenuItem(mn);
            uiElement.RightTapped += (s, e) => { e.Handled = true;

            };
           

           
            this.UIElement = uiElement;

            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.Out, Label = "Output", WorkFlowItem = this });
        }

        public async Task Start(params object[] args)
        {
            await Run("Sample");
        }

    }
}
