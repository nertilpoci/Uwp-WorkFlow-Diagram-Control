using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkFlow.Enums;
using WorkFlow.Interface;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using WorkFlow.Controls.Workflow;
using System;
using WorkFlow.ViewModels;

namespace WorkFlow.Impl
{
   public abstract class BaseWorkFlowElement: UserControl
    {
        FrameworkElement parent;
        public BaseWorkFlowElement(FrameworkElement parent)
        {
            this.parent = parent;
            ItemContent = new WorkFlowItemViewModel();
        }
        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        public FrameworkElement Element { get; set; }
        public IWorkFlowItemContent ItemContent { get; set; }
        private float magic = 8;
        public void Move(Point point,double width, double height)
        {
            Canvas.SetLeft(Element, point.X - width / 2);
            Canvas.SetTop(Element, point.Y - height / 2);

            foreach (var connector in this.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var path = line as Line;
                    var ui = connector as UserControl;
                    var transform = ui.TransformToVisual(parent);
                    Point absolutePosition = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
                    absolutePosition.X += ui.ActualWidth / 2;
                    absolutePosition.Y += ui.ActualHeight / 2;
                    if (connector.Type == Interface.ConnectorType.In)
                    {
                        line.End.Point = absolutePosition;
                        path.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }
                    else
                    {
                        line.Start.Point = absolutePosition;
                        path.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }

                }
            }
        }
        private Point _position;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Point Position { get { return GetPosition(); } set { _position = value; } }
        public InputOutputConnectorPosition ConnectorLayout { get; set; } = InputOutputConnectorPosition.RightLeft;      
        public Point GetPosition()
        {
            return Element.TransformToVisual(parent).TransformPoint(new Point(0, 0)); ;
        }

       
    }

    public abstract class ExecutableNodeBase:BaseWorkFlowElement
    {
        public ExecutableNodeBase(FrameworkElement parent) : base(parent) { }
        IConnector[] _connectors;
        private bool _isExecuting;
        public bool IsExecuting { get { return _isExecuting; } set { _isExecuting = value; OnPropertyChanged(); } }
        public Func<object[], object> OnExecuteAction { get; set; }
        public async Task Run(IConnector[] connectors, params object[] args)
        {
            _connectors = connectors;
            IsExecuting = true;
            await Task.Run(() => CallNextItem(OnExecuteAction(args)));
        }
       
        private void CallNextItem(object input)
        {
            _connectors.Where(z => z.Type == ConnectorType.Out).ToList().ForEach(connector => {
                Parallel.ForEach(connector.Lines, line => {
                    if (line.End.WorkFlowItem is IExecutableNode) ((IExecutableNode)line.End.WorkFlowItem).Run(input);
                });
            });
        }
    }

    

}
