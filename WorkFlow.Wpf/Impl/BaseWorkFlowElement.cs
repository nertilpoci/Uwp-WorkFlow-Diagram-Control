using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using WorkFlow.Controls.Workflow;
using System;
using System.Windows;
using System.Windows.Controls;
using Workflow.Common.Interface;
using WorkFlow.Wpf.Controls.Workflow;
using WorkFlow.Wpf.ViewModels;
using Workflow.Common.Models;
using Workflow.Common.Enums;
using WorkFlow.Extensions;
using WorkFlow.Wpf.Extensions;
using Workflow.Common.ViewModels;
using Workflow.Common.Extensions;

namespace WorkFlow.Wpf.Impl
{
    public abstract class BaseWorkFlowElement: UserControl,IWorkFlowItem, INotifyPropertyChanged
    {
        FrameworkElement parent;
        public BaseWorkFlowElement(FrameworkElement parent)
        {
            this.parent = parent;
            ItemContent = new WorkFlowItemContentBase();
            ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();
        }
        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        public object Element { get; set; }
        public IWorkFlowItemContent ItemContent { get; set; }
        private float magic = 8;
     
        private WorkFlowPoint _position;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public WorkFlowPoint Position { get { return GetPosition().CreateWorkFlowPoint(); } set { _position = value; } }
        public Point GetPosition()
        {
            return Element.ToFrameworkElement().TransformToVisual(parent).Transform(new Point(0, 0)); ;
        }

        public IConnector AddConnector(IConnector connector)
        {
            return this.AddWorkFlowConnector(connector);
        }

        public void Move(WorkFlowPoint point)
        {
            var width = Element.ToFrameworkElement().ActualWidth;
            var height = Element.ToFrameworkElement().ActualHeight;
            Canvas.SetLeft(Element.ToFrameworkElement(), point.X - width / 2);
            Canvas.SetTop(Element.ToFrameworkElement(), point.Y - height / 2);
          

            Func<IConnector, WorkFlowPoint> getPostion = (x) => {
                var uiControl = x as UserControl;
                var transform = uiControl.TransformToVisual(parent);
                Point absolutePosition = transform.Transform(new Point(0, 0));
                absolutePosition.X += uiControl.ActualWidth / 2;
                absolutePosition.Y += uiControl.ActualHeight / 2;
                return absolutePosition.CreateWorkFlowPoint();
            };
            this.MoveItem(point, width, height, getPostion);
        }
    }

    public abstract class ExecutableNodeBase:BaseWorkFlowElement, IExecutableNode
    {
        public ExecutableNodeBase(FrameworkElement parent) : base(parent) { }
        private bool _isExecuting;
        public bool IsExecuting { get { return _isExecuting; } set { _isExecuting = value; OnPropertyChanged(); } }
        public Func<object, Task<object>> OnExecuteAction { get; set; }
      

        public async Task Run(object input = null)
        {
            await this.RunNode(input);
        }
    }

    

}
