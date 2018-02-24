using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Workflow.Common.Implementation;
using WorkFlow.Extensions;
using WorkFlow.WorkFlowItems.Controls;

namespace WorkFlow.WorkFlowItems.Items
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
            _uiControl.Ellipse.Fill = new SolidColorBrush(mouseOverColor.HexToColor());
        }

        public override void MouseOut()
        {
            _uiControl.Ellipse.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public override void SetCanConnectUi(bool reset = false)
        {
            _uiControl.Ellipse.Fill = reset ? new SolidColorBrush(normalColor.HexToColor()) : new SolidColorBrush(canConnectColor.HexToColor());
        }
    }
}
