using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Workflow.Common.Enums;
using Workflow.Common.Implementation;
using Workflow.Common.Interface;
using Workflow.Common.ViewModels;
using WorkFlow.Wpf.Impl;
using WorkFlow.Wpf.WorkFlowItems.Controls;

namespace WorkFlow.Wpf.WorkFlowItems.Items
{
    public class TriggerWorkFlowItem: WpfExecutableNodeBase, ITriggerNode
    {
        FrameworkElement _parent;
        public TriggerWorkFlowItem(FrameworkElement parent) : base(parent)
        {

            _parent = parent;

            this.ItemContent = new WorkFlowItemContentBase();
            this.ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();

            OnExecuteAction = async input => { await Task.Delay(5000); return input;  };
           
            var uiElement = new WorkFlowItemControl(_parent) { DataContext = this };

            uiElement.MouseRightButtonDown += (s, e) => { e.Handled = true;


            };
            MenuItem menu =new MenuItem() { Header="Start"};
            menu.Click += async (s, e) => { await Start(); };
            uiElement.ContextMenu = new ContextMenu();
            uiElement.ContextMenu.Items.Add(menu);

            this.UIElement = uiElement;

            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.Out, Label = "Output", WorkFlowItem = this });
        }

        public async Task Start(params object[] args)
        {
            await Run("Sample");
        }

    }
}
