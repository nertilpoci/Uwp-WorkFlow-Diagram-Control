using System.Windows;
using System.Windows.Controls;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Wpf.WorkFlowItems.Controls
{
    public sealed partial class WorkFlowItemControl : UserControl, IUIElement
    {
      FrameworkElement _parent;
      public WorkFlowItemControl(FrameworkElement parent)
        {
            InitializeComponent();
            _parent = parent;
          
        }
        public double ItemWidth => this.ActualWidth;

        public double ItemHeight => this.ActualHeight;
        public T GetUiElement<T>() where T : class
        {
            return this as T;
        }

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
