using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Wpf.WorkFlowItems.Controls
{
    public sealed partial class ConnectorControl : UserControl,IUIElement
    {
        FrameworkElement _parent;
        public ConnectorControl(FrameworkElement parent)
        {
            this.InitializeComponent();
            _parent = parent;
        }

        public Ellipse Ellise => anchor;
        public T GetUiElement<T>() where T : class
        {
            return this as T;
        }

        public double ItemWidth =>this.ActualWidth;

        public double ItemHeight => this.ActualHeight;

        public WorkFlowPoint GetPosition()
        {
            var transform = this.TransformToVisual(_parent);
            return transform.Transform(new Point(0, 0)).ToWorkFlowPoint();

        }

        public void SetPosition(WorkFlowPoint point)
        {
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
        }
    }
}
