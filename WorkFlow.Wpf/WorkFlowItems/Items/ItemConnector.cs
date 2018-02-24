using System.Windows;
using System.Windows.Media;
using Workflow.Common.Implementation;
using WorkFlow.Extensions;
using WorkFlow.Wpf.WorkFlowItems.Controls;

namespace WorkFlow.Wpf.WorkFlowItems.Items
{
    public class ItemConnector: ConnectorBase
    {
        private ConnectorControl _uiControl;
        public ItemConnector(FrameworkElement parent,double width,double height) {

            _uiControl = new ConnectorControl(parent);
            _uiControl.Width = width;
            _uiControl.Height = height;
            UIElement = _uiControl;

        }

        public override void MouseIn()
        {
            _uiControl.Ellise.Fill = new SolidColorBrush(mouseOverColor.HexToColor());
        }

        public override void MouseOut()
        {
            _uiControl.Ellise.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public override void SetCanConnectUi(bool reset = false)
        {
            _uiControl.Ellise.Fill = reset ? new SolidColorBrush(normalColor.HexToColor()) : new SolidColorBrush(canConnectColor.HexToColor());
        }
    }
}
