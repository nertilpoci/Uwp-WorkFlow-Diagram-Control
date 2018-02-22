
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WorkFlow.Enums;
using WorkFlow.Extensions;
using WorkFlow.Interface;
using WorkFlow.Models;

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
         
            LoopDetected += async (s, e) =>{

                var dialog = new MessageDialog("This Connection creates a loop, loops are not allowed");
                await dialog.ShowAsync();
            };
            NewNodeCommand = new RelayCommand<NodeType>(CreateNode);
        }

        private void CreateNode(NodeType nodeType)
        {//TODO better node generation, DI
            switch (nodeType)
            {
                case NodeType.Trigger:
                    var trigger=  CreateNewNode(typeof(TriggerWorkflowItem), "Sample Trigger", "Test trigger node");
                    WorkFlowItems.Add(trigger);
                    editorCanvas.Children.Add(trigger.Element);
                    Canvas.SetLeft(trigger.Element, _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(trigger.Element, _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Action:
                    var action = CreateNewNode(typeof(ActionWorkFlowItem), "Sample Action", "Test action node");
                    WorkFlowItems.Add(action);
                    editorCanvas.Children.Add(action.Element);
                    Canvas.SetLeft(action.Element, _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(action.Element, _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Result:
                    var result = CreateNewNode(typeof(ResultWorkflowItem), "Sample Result", "Test result node");
                    WorkFlowItems.Add(result);
                    editorCanvas.Children.Add(result.Element);
                    Canvas.SetLeft(result.Element, _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(result.Element, _canvasLastRightTappedPoint.Y);
                    break;
                default:
                    break;
            }
        }

    
     
        private void SetScrollViewerEvents()
        {
            Point scrollPoint;
            scroller.PointerPressed += (s, e) =>
            {
                if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
                {
                    e.Handled = true;
                    var sender = s as FrameworkElement;
                    sender.CapturePointer(e.Pointer);
                    scrollPoint = e.GetCurrentPoint(editorCanvas).Position;
                }
            };

            scroller.PointerMoved += (s, e) =>
            {
                var sender = s as FrameworkElement;
                if (sender.PointerCaptures != null && sender.PointerCaptures.Any())
                {
                    e.Handled = true;
                    var newPoint = e.GetCurrentPoint(scroller).Position;
                    var vOffset = scroller.VerticalOffset + (scrollPoint.Y - newPoint.Y);
                    var hOffset = scroller.HorizontalOffset + (scrollPoint.X - newPoint.X);
                    scroller.ChangeView(hOffset, vOffset, scroller.ZoomFactor);
                }
            };

            scroller.PointerReleased += (s, e) =>
            {
                var sender = s as FrameworkElement;
                sender.ReleasePointerCaptures();
            };
        }
        private Point _canvasLastRightTappedPoint=new Point(100,100);
        private void SetCanvasEvents()
        {
               editorCanvas.IsRightTapEnabled = true;
            editorCanvas.RightTapped += (s, e) =>
            {
                _canvasLastRightTappedPoint = e.GetPosition(editorCanvas);
                

            };

            editorCanvas.PointerReleased += (s, e) =>
            {
                ConnectionCreateCleanup();
            };

            editorCanvas.PointerMoved += (s, e) => {
                if (MovingLine == null) return;
                MovingLine.DrawPath(MovingLine.Start.Point, e.GetCurrentPoint(editorCanvas).Position);
            };

        }
        private void ConnectionCreateCleanup()
        {
            if (MovingLine == null) return;
            this.editorCanvas.Children.Remove(MovingLine.Element);
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
            item.Element.RenderTransform = new TranslateTransform();
            item.ItemContent.ItemContentContext.Title =title;
            item.ItemContent.ItemContentContext.Description = description;

            foreach (var connector in item.Connectors)
            {
                connector.Element.PointerEntered += (s, e) => {
                    connector.MouseIn();
                };
                item.Element.PointerExited += (s, e) => {
                    connector.MouseOut();
                };

                if (connector.Type == Interface.ConnectorType.Out)
                {
                    connector.Element.PointerPressed += (s, e) =>
                    {
                        e.Handled = true;
                        connector.Point = e.GetCurrentPoint(editorCanvas).Position;

                        MovingLine =CreateLine(connector, null);
                        connector.Lines.Add(MovingLine);
                        editorCanvas.Children.Add(MovingLine.Element);
                        SetResetConnectorCanConnect(MovingLine,WorkFlowItems);
                    };

                    connector.Element.PointerReleased += (s, e) =>
                    {
                        ConnectionCreateCleanup();
                    };
                }
                else
                {
                    connector.Element.PointerReleased += (s, e) =>
                    {

                        if (MovingLine != null)
                        {
                            e.Handled = true;

                            if(!connector.CanConnect(MovingLine))
                            {
                                ConnectionCreateCleanup();
                                return;
                            }

                            if (IsLoop(MovingLine.Start,connector))
                            {
                                OnLoopDetected(new WorkFlowLoopEventModel(MovingLine.Start, connector));

                                if (!AllowLoops)
                                {
                                    ConnectionCreateCleanup();
                                    return;
                                }
                              
                            }

                            connector.Point = e.GetCurrentPoint(editorCanvas).Position;
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


            item.Element.PointerPressed += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.CapturePointer(e.Pointer);
            };

            item.Element.PointerReleased += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.ReleasePointerCapture(e.Pointer);
                ConnectionCreateCleanup();
            };

            item.Element.PointerMoved += (s, e) =>
            {
                editorCanvas.Width = editorCanvas.ActualWidth;
                editorCanvas.Height = editorCanvas.ActualHeight;
                scroller.Width = scroller.ActualWidth;
                scroller.Height = scroller.ActualHeight;

                e.Handled = true;
                var sender = s as FrameworkElement;
                if (sender.PointerCaptures != null && sender.PointerCaptures.Any())
                {
                    var point = e.GetCurrentPoint(editorCanvas).Position;

                    if (point.X >= editorCanvas.Width - ItemWidth)
                    {
                        editorCanvas.Width += 50; scroller.ChangeView(editorCanvas.Width, null, scroller.ZoomFactor);
                    }
                    if (point.Y >= editorCanvas.Height - ItemHeight) { editorCanvas.Height += 50; scroller.ChangeView(null, editorCanvas.Height, scroller.ZoomFactor); }


                    if (point.X >= scroller.ActualWidth + scroller.HorizontalOffset - ItemWidth)
                    {
                        scroller.ChangeView(scroller.HorizontalOffset + 50, null, scroller.ZoomFactor);
                    }
                    if (point.Y >= scroller.VerticalOffset + scroller.ActualHeight - ItemHeight) { scroller.ChangeView(null, scroller.VerticalOffset + 50, scroller.ZoomFactor); }


                    if (point.X < scroller.HorizontalOffset + ItemWidth/2 )
                    {
                        scroller.ChangeView(scroller.HorizontalOffset - 20, null, scroller.ZoomFactor);
                    }
                    if (point.Y <scroller.VerticalOffset + ItemHeight/2) { scroller.ChangeView(null, scroller.VerticalOffset - 20, scroller.ZoomFactor); }



                    item.Move(point);
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
            line.RightTapped += Line_RightTapped;
            line.LineDeleted += Line_LineDeleted;
            line.Element.PointerEntered += (s,e)=> { line.MouseIn(); };
            line.Element.PointerExited += (s, e) => { line.MouseOut(); };
            return line;
        }
        public ICommand NewNodeCommand { get; set; }
        private void Line_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Line_LineDeleted(object sender, ILine e)
        {
            this.editorCanvas.Children.Remove(e.Element);
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
            path.Data = geo;
            Canvas.SetZIndex(path, -2);
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


