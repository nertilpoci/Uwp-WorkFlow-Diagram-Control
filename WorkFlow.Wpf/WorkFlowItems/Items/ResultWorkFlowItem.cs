using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Workflow.Common.Enums;
using Workflow.Common.Implementation;
using Workflow.Common.ViewModels;
using WorkFlow.Wpf.Impl;
using WorkFlow.Wpf.WorkFlowItems.Controls;

namespace WorkFlow.Wpf.WorkFlowItems.Items
{
    public class ResultWorkFlowItem : WpfExecutableNodeBase
    {
        FrameworkElement _parent;
        public ResultWorkFlowItem(FrameworkElement parent) : base(parent)
        {
            _parent = parent;
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;

            this.ItemContent = new WorkFlowItemContentBase();

            this.ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();

            OnExecuteAction = async input => { ShowNotification("Workflow Notification", input?.ToString()); return null;  };

            this.UIElement = new WorkFlowItemControl(_parent) { DataContext = this };
            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.In, Label = "Input", WorkFlowItem = this });

        }
        private readonly NotifyIcon _notifyIcon;


        public void ShowNotification(string title,string message)
        {
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);
        }
    }
}
