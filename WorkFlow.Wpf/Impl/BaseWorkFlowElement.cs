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

namespace WorkFlow.Wpf.Impl
{
    public abstract class BaseWorkFlowElement: UserControl
    {
        FrameworkElement parent;
        public BaseWorkFlowElement(FrameworkElement parent)
        {
            this.parent = parent;
            ItemContent = new WorkFlowItemContentBase();
            ItemContent.ItemContentContext = new WorkFlowItemContentViewModel();
        }
        public IConnector AddConnector(IConnector connector)
        {
            Connectors.Add(connector);
            ItemContent.AddConnector(connector);
            return connector;
        }
        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        public object Element { get; set; }
        public IWorkFlowItemContent ItemContent { get; set; }
        private float magic = 8;
        public void Move(WorkFlowPoint point)
        {
            var width = Element.ToFrameworkElement().ActualWidth;
            var height = Element.ToFrameworkElement().ActualHeight;
            Canvas.SetLeft(Element.ToFrameworkElement(), point.X - width / 2);
            Canvas.SetTop(Element.ToFrameworkElement(), point.Y - height / 2);

            foreach (var connector in this.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var path = line as Line;
                    var ui = connector as UserControl;
                    var transform = ui.TransformToVisual(parent);

                    Point absolutePosition = transform.Transform(new Point(0, 0));
                    absolutePosition.X += ui.ActualWidth / 2;
                    absolutePosition.Y +=ui.ActualHeight / 2;
                    if (connector.Type == ConnectorType.In)
                    {
                        line.End.Point = absolutePosition.CreateWorkFlowPoint();
                        path.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }
                    else
                    {
                        line.Start.Point = absolutePosition.CreateWorkFlowPoint();
                        path.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }

                }
            }
        }
        private WorkFlowPoint _position;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public WorkFlowPoint Position { get { return GetPosition().CreateWorkFlowPoint(); } set { _position = value; } }
        public Point GetPosition()
        {
            return Element.ToFrameworkElement().TransformToVisual(parent).Transform(new Point(0, 0)); ;
        }

       
    }

    public abstract class ExecutableNodeBase:BaseWorkFlowElement
    {
        public ExecutableNodeBase(FrameworkElement parent) : base(parent) { }
        private bool _isExecuting;
        public bool IsExecuting { get { return _isExecuting; } set { _isExecuting = value; OnPropertyChanged(); } }
        public Func<object, Task<object>> OnExecuteAction { get; set; }
        public async Task Run( object input=null)
        {
           //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { IsExecuting = true; });
            var result= await Task.Run(() => OnExecuteAction(input));
           //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { IsExecuting = false; });
            Task.Run(() => CallNextItem(result)); // fire and forget
        }
       
        private void CallNextItem(object input)
        {
            Connectors.Where(z => z.Type == ConnectorType.Out).ToList().ForEach(connector => {
                Parallel.ForEach(connector.Lines, line => {
                    if (line.End.WorkFlowItem is IExecutableNode) ((IExecutableNode)line.End.WorkFlowItem).Run(input);
                });
            });
            
        }
    }

    

}
