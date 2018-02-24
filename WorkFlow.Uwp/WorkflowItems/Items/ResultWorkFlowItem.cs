using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Workflow.Common.Enums;
using Workflow.Common.ViewModels;
using WorkFlow.Impl;
using WorkFlow.WorkFlowItems.Controls;

namespace WorkFlow.WorkFlowItems.Items
{
    public class ResultWorkFlowItem : UWPExecutableNodeBase
    {
        public ResultWorkFlowItem(FrameworkElement parent) : base(parent)
        {
            this.ItemContent = new WorkFlowItemContentBase();

            this.ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();

            OnExecuteAction = async input => { ShowToastNotification("WorkFlow notification",input?.ToString()) ;  return null;  };

            this.UIElement = new WorkFlowItemControl(parent) { DataContext = this };
            AddConnector(new ItemConnector(parent, 25, 25) { Type = ConnectorType.In, Label = "Input", WorkFlowItem = this });

        }
        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(10);
            ToastNotifier.Show(toast);
        }

    }
}
