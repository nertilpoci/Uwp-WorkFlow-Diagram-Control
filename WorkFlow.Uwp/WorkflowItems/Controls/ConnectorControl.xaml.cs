using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.WorkFlowItems.Controls
{
    public sealed partial class ConnectorControl : UserControl,IUIElement
    {
        protected string normalColor = "#005b96";
        protected string mouseOverColor = "#03396c";
        protected string canConnectColor = "#83AA30";
        FrameworkElement _parent;
        public ConnectorControl(FrameworkElement parent)
        {
            this.InitializeComponent();
            _parent = parent;
        }
        public Ellipse Ellipse => anchor;

        public double ItemWidth => this.ActualWidth;

        public double ItemHeight => this.ActualHeight;

        public WorkFlowPoint GetPosition()
        {
            var transform = this.TransformToVisual(_parent);
            return transform.TransformPoint(new Point(0, 0)).CreateWorkFlowPoint();

        }



        public T GetUiElement<T>() where T : class
        {
            return this as T;
        }

        public void SetPosition(WorkFlowPoint point)
        {
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
        }
    }
}
