using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;
using WorkFlow.Wpf.Controls.Workflow;
using WorkFlow.Wpf.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class WorkFlowEditor : UserControl,IWorkFlowEditor
    {
        int ItemHeight = 100;
        int ItemWidth = 200;
        private IList<IWorkFlowItem> WorkFlowItems = new List<IWorkFlowItem>();
        public WorkFlowEditor()
        {
            InitializeComponent();
            this.DataContext = this;

           
            SetScrollViewerEvents();
            SetCanvasEvents();
         
            LoopDetected +=  (s, e) =>{

                MessageBox.Show("This Connection creates a loop, loops are not allowed");
            };
            NewNodeCommand = new RelayCommand<NodeType>(CreateNode);
        }

        private void CreateNode(NodeType nodeType)
        {//TODO better node generation, DI
            switch (nodeType)
            {
                case NodeType.Trigger:
                    var trigger = CreateNewNode(typeof(TriggerWorkFlowItem), "Sample Trigger", "Test trigger node");
                    WorkFlowItems.Add(trigger);
                    editorCanvas.Children.Add(trigger.Element.ToFrameworkElement());
                    Canvas.SetLeft(trigger.Element.ToFrameworkElement(), _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(trigger.Element.ToFrameworkElement(), _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Action:
                    var action = CreateNewNode(typeof(ActionWorkFlowItem), "Sample Action", "Test action node");
                    WorkFlowItems.Add(action);
                    editorCanvas.Children.Add(action.Element.ToFrameworkElement());
                    Canvas.SetLeft(action.Element.ToFrameworkElement(), _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(action.Element.ToFrameworkElement(), _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Result:
                    var result = CreateNewNode(typeof(ResultWorkFlowItem), "Sample Result", "Test result node");
                    WorkFlowItems.Add(result);
                    editorCanvas.Children.Add(result.Element.ToFrameworkElement());
                    Canvas.SetLeft(result.Element.ToFrameworkElement().ToFrameworkElement(), _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(result.Element.ToFrameworkElement(), _canvasLastRightTappedPoint.Y);
                    break;
                default:
                    break;
            }
        }

    
     
        private void SetScrollViewerEvents()
        {
            //Point scrollPoint;
            //scroller.PointerPressed += (s, e) =>
            //{
            //    if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            //    {
            //        e.Handled = true;
            //        var sender = s as FrameworkElement;
            //        sender.CapturePointer(e.Pointer);
            //        scrollPoint = e.GetCurrentPoint(editorCanvas).Position;
            //    }
            //};

            //scroller.PointerMoved += (s, e) =>
            //{
            //    var sender = s as FrameworkElement;
            //    if (sender.PointerCaptures != null && sender.PointerCaptures.Any())
            //    {
            //        e.Handled = true;
            //        var newPoint = e.GetCurrentPoint(scroller).Position;
            //        var vOffset = scroller.VerticalOffset + (scrollPoint.Y - newPoint.Y);
            //        var hOffset = scroller.HorizontalOffset + (scrollPoint.X - newPoint.X);
            //        scroller.ChangeView(hOffset, vOffset, scroller.ZoomFactor);
            //    }
            //};

            //scroller.PointerReleased += (s, e) =>
            //{
            //    var sender = s as FrameworkElement;
            //    sender.ReleasePointerCaptures();
            //};
        }
        private Point _canvasLastRightTappedPoint=new Point(100,100);
        private void SetCanvasEvents()
        {
            editorCanvas.MouseRightButtonUp += (s, e) =>
            {
                _canvasLastRightTappedPoint = e.GetPosition(editorCanvas);
                

            };

            editorCanvas.ToFrameworkElement().MouseLeftButtonUp += (s, e) =>
            {
                ConnectionCreateCleanup();
            };

            editorCanvas.ToFrameworkElement().MouseMove += (s, e) =>
            {
                if (MovingLine == null) return;
                MovingLine.DrawPath(MovingLine.Start.Point, e.GetPosition(editorCanvas).CreateWorkFlowPoint());
            };

        }
        private void ConnectionCreateCleanup()
        {
            if (MovingLine == null) return;
            this.editorCanvas.Children.Remove(MovingLine.Element.ToFrameworkElement());
            MovingLine.Start.Lines.Remove(MovingLine);
            MovingLine = null;
            SetResetConnectorCanConnect(null, WorkFlowItems);
        }
        ILine MovingLine;

        public event EventHandler<WorkFlowLoopEventModel> LoopDetected;

        public bool AllowLoops { get; set; }

        private  void OnLoopDetected(WorkFlowLoopEventModel e)
        {
            LoopDetected?.Invoke(this, e);
        }
        private void SetResetConnectorCanConnect(ILine line,IList<IWorkFlowItem> items)
        {
            foreach (var wE in items)
            {
                foreach (var c in wE.Connectors)
                {
                    c.SetCanConnectUi(c.CanConnect(MovingLine) == false);
                }
            }
        }
        private IWorkFlowItem CreateNewNode(Type type, string title, string description)
        {
            IWorkFlowItem item = (IWorkFlowItem)Activator.CreateInstance(type, editorCanvas);
            item.ItemContent.ItemContentContext.Title =title;
            item.ItemContent.ItemContentContext.Description = description;

            foreach (var connector in item.Connectors)
            {
                connector.Element.ToFrameworkElement().MouseEnter += (s, e) =>
                {
                    connector.MouseIn();
                };
                item.Element.ToFrameworkElement().MouseLeave += (s, e) =>
                {
                    connector.MouseOut();
                };

                if (connector.Type == ConnectorType.Out)
                {
                    connector.Element.ToFrameworkElement().MouseLeftButtonDown += (s, e) =>
                    {
                        e.Handled = true;
                        connector.Point = e.GetPosition(editorCanvas).CreateWorkFlowPoint();

                        MovingLine = CreateLine(connector, null);
                        connector.Lines.Add(MovingLine);
                        editorCanvas.Children.Add(MovingLine.Element.ToFrameworkElement());
                        SetResetConnectorCanConnect(MovingLine, WorkFlowItems);
                    };

                    connector.Element.ToFrameworkElement().MouseLeftButtonUp += (s, e) =>
                    {
                        ConnectionCreateCleanup();
                    };
                }
                else
                {
                    connector.Element.ToFrameworkElement().MouseLeftButtonUp += (s, e) =>
                    {

                        if (MovingLine != null)
                        {
                            e.Handled = true;

                            if (!connector.CanConnect(MovingLine))
                            {
                                ConnectionCreateCleanup();
                                return;
                            }

                            if (IsLoop(MovingLine.Start, connector))
                            {
                                OnLoopDetected(new WorkFlowLoopEventModel(MovingLine.Start, connector));

                                if (!AllowLoops)
                                {
                                    ConnectionCreateCleanup();
                                    return;
                                }

                            }

                            connector.Point = e.GetPosition(editorCanvas).CreateWorkFlowPoint();
                            MovingLine.End = connector;
                            MovingLine.DrawPath(MovingLine.Start.Point, MovingLine.End.Point);
                            connector.Lines.Add(MovingLine);

                            MovingLine = new Line();
                            MovingLine = null;
                            SetResetConnectorCanConnect(null, WorkFlowItems);

                        }
                    };
                }
            }


            item.Element.ToFrameworkElement().MouseLeftButtonDown += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.CaptureMouse();
            };

            item.Element.ToFrameworkElement().MouseLeftButtonUp += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.ReleaseMouseCapture();
                ConnectionCreateCleanup();
            };

            item.Element.ToFrameworkElement().MouseMove += (s, e) =>
            {
                editorCanvas.Width = editorCanvas.ActualWidth;
                editorCanvas.Height = editorCanvas.ActualHeight;
                scroller.Width = scroller.ActualWidth;
                scroller.Height = scroller.ActualHeight;

                e.Handled = true;
                var sender = s as FrameworkElement;
                if (sender.IsMouseCaptured )
                {
                    var point = e.GetPosition(editorCanvas);

                    if (point.X >= editorCanvas.Width - ItemWidth)
                    {
                        editorCanvas.Width += 50; scroller.ScrollToHorizontalOffset(editorCanvas.Width);
                    }
                    if (point.Y >= editorCanvas.Height - ItemHeight) { editorCanvas.Height += 50; scroller.ScrollToVerticalOffset( editorCanvas.Height); }


                    if (point.X >= scroller.ActualWidth + scroller.HorizontalOffset - ItemWidth)
                    {
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 50 );
                    }
                    if (point.Y >= scroller.VerticalOffset + scroller.ActualHeight - ItemHeight) { scroller.ScrollToVerticalOffset(scroller.VerticalOffset + 50 ); }


                    if (point.X < scroller.HorizontalOffset + ItemWidth / 2)
                    {
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 20);
                    }
                    if (point.Y < scroller.VerticalOffset + ItemHeight / 2) { scroller.ScrollToVerticalOffset(scroller.VerticalOffset - 20); }



                    item.Move(point.CreateWorkFlowPoint());
                    MakeCanvasFit((int)this.ActualWidth, (int)this.ActualHeight);
                }
            };
            return item;
          
        }
        public  ILine CreateLine(IConnector start, IConnector end)
        {
            var line = new Line

            {
                Start = start,
                End = end
            };
            //line.RightTapped += Line_RightTapped;
            //line.LineDeleted += Line_LineDeleted;
            //line.Element.PointerEntered += (s,e)=> { line.MouseIn(); };
            //line.Element.PointerExited += (s, e) => { line.MouseOut(); };
            return line;
        }
        public ICommand NewNodeCommand { get; set; }
        private void Line_RightTapped(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        private void Line_LineDeleted(object sender, ILine e)
        {
            this.editorCanvas.Children.Remove(e.Element.ToFrameworkElement());
        }

        private void MakeCanvasFit(int minX, int minY)
        {
            var xPadding = 300;
            var yPadding = 200;
            var maxX = this.WorkFlowItems.Max(z => z.Position.X);
            var maxY = this.WorkFlowItems.Max(z => z.Position.Y);
            if(maxX+xPadding>minX) editorCanvas.Width = maxX + xPadding;
            if(maxY+yPadding>minY) editorCanvas.Height = maxY + yPadding;
        }
      
        private void DrawPath(Point source, Point destination, Line path)
        {
            float magic = 8;
            var segments = new List<PathSegment>();
            if (destination.X > source.X)
            {
                Point c1 = new Point(source.X + (magic - 1) * (destination.X - source.X) / magic, source.Y + (destination.Y - source.Y) / magic);
                Point c2 = new Point(source.X + (destination.X - source.X) / magic, source.Y + (magic - 1) * (destination.Y - source.Y) / magic);
                var bs = new BezierSegment();
                bs.Point1 = c1;
                bs.Point2 = c2;
                bs.Point3 = destination;
                segments.Add(bs);
            }
            else
            {
                double distanceX = Math.Abs(source.X - destination.X) / 3;
                Point c1 = new Point(source.X + Math.Min((magic - 1) * (source.X - destination.X) / magic, distanceX) + 20, source.Y + (magic - 1) * (destination.Y - source.Y) / magic);
                Point c2 = new Point(destination.X - Math.Min((magic - 1) * (source.X - destination.X) / magic, distanceX) - 20, destination.Y - (magic - 1) * (destination.Y - source.Y) / magic);
                var bs = new BezierSegment();
                bs.Point1 = c1;
                bs.Point2 = c2;
                bs.Point3 = destination;
                segments.Add(bs);
            }

            var geo = new PathGeometry();
            var pf = new PathFigure() { StartPoint = source, IsClosed = false };
            foreach (var s in segments)
            {
                pf.Segments.Add(s);
            }
            geo.Figures.Add(pf);
            ((Path)path.Element).Data = geo;
            Canvas.SetZIndex(((Path)path.Element), -2);
        }
        private bool IsLoop(IConnector start, IConnector end)
        {
            foreach (var item in end.WorkFlowItem.Connectors.Where(z=>z.Type==ConnectorType.Out))
            {
                foreach (var line in item.Lines)
                {
                    if (line.End.WorkFlowItem == start.WorkFlowItem) return true;
                    return IsLoop(start, line.End);
                }
            }
            return false;
        }


    }
   
}


