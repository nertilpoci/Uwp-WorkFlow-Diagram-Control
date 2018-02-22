using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using WorkFlow.Controls.Workflow;
using System;
using WorkFlow.ViewModels;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using WorkFlow.Extensions;
using Workflow.Common.Models;
using WorkFlow.Wpf.Extensions;

namespace WorkFlow.Impl
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
            var width = ((FrameworkElement)Element).ActualWidth;
            var height = ((FrameworkElement)Element).ActualHeight;
            Canvas.SetLeft((FrameworkElement)Element, point.X - width / 2);
            Canvas.SetTop((FrameworkElement)Element, point.Y - height / 2);

            foreach (var connector in this.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var path = line as Line;
                    var ui = connector as UserControl;
                    var transform = ui.TransformToVisual(parent);

                    Point absolutePosition = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
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
            return ((FrameworkElement)Element).TransformToVisual(parent).TransformPoint(new Point(0, 0)); ;
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
           await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { IsExecuting = true; });
            var result= await Task.Run(() => OnExecuteAction(input));
           await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { IsExecuting = false; });
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
