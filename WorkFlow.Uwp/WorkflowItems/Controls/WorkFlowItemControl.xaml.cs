using Windows.UI.Xaml;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.WorkFlowItems.Controls
{
    public sealed partial class WorkFlowItemControl :UserControl, IUIElement
    {
        FrameworkElement _parent;
        public WorkFlowItemControl(FrameworkElement parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        public void AddContextMenuItem(MenuFlyoutItem item)
        {
            contextMenu.Items.Add(item);
        }
        public T GetUiElement<T>() where T : class
        {
            return this as T;
        }

        public double ItemWidth => this.ActualWidth;

        public double ItemHeight =>this.ActualHeight;

        public WorkFlowPoint GetPosition()
        {
            var transform = this.TransformToVisual(_parent);
            return transform.TransformPoint(new Point(0, 0)).CreateWorkFlowPoint();

        }

        public void SetPosition(WorkFlowPoint point)
        {
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
        }
    }
}
