using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;
using WorkFlow.Wpf.Extensions;
using WorkFlow.Wpf.WorkFlowItems.Items;

namespace WorkFlow.Wpf.WorkFlowItems.Controls
{
    /// <summary>
    /// Interaction logic for WorkFlowEditor.xaml
    /// </summary>
    public partial class WorkFlowEditor : UserControl
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

            LoopDetected += (s, e) => {

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
                     var uiel = trigger.UIElement as UserControl;
                   
                    editorCanvas.Children.Add(uiel);
                    Canvas.SetLeft(uiel, _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(uiel, _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Action:
                    var action = CreateNewNode(typeof(ActionWorkFlowItem), "Sample Action", "Test action node");
                    WorkFlowItems.Add(action);
                    editorCanvas.Children.Add(action.UIElement.GetUiElement<FrameworkElement>());
                    Canvas.SetLeft(action.UIElement.GetUiElement<FrameworkElement>(), _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(action.UIElement.GetUiElement<FrameworkElement>(), _canvasLastRightTappedPoint.Y);
                    break;
                case NodeType.Result:
                    var result = CreateNewNode(typeof(ResultWorkFlowItem), "Sample Result", "Test result node");
                    WorkFlowItems.Add(result);
                    editorCanvas.Children.Add(result.UIElement.GetUiElement<FrameworkElement>());
                    Canvas.SetLeft(result.UIElement.GetUiElement<FrameworkElement>().ToFrameworkElement(), _canvasLastRightTappedPoint.X);
                    Canvas.SetTop(result.UIElement.GetUiElement<FrameworkElement>(), _canvasLastRightTappedPoint.Y);
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
        private Point _canvasLastRightTappedPoint = new Point(100, 100);
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
                MovingLine.DrawPath(MovingLine.Start.Point, e.GetPosition(editorCanvas).ToWorkFlowPoint());
            };

        }
        private void ConnectionCreateCleanup()
        {
            if (MovingLine == null) return;
            this.editorCanvas.Children.Remove(MovingLine.UIElement.GetUiElement<FrameworkElement>());
            MovingLine.Start.Lines.Remove(MovingLine);
            MovingLine = null;
            SetResetConnectorCanConnect(null, WorkFlowItems);
        }
        ILine MovingLine;

        public event EventHandler<WorkFlowLoopEventModel> LoopDetected;

        public bool AllowLoops { get; set; }

        private void OnLoopDetected(WorkFlowLoopEventModel e)
        {
            LoopDetected?.Invoke(this, e);
        }
        private void SetResetConnectorCanConnect(ILine line, IList<IWorkFlowItem> items)
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
            item.ItemContent.ItemContentContext.Title = title;
            item.ItemContent.ItemContentContext.Description = description;

            item.UIElement.GetUiElement<FrameworkElement>().Width = 200;
            item.UIElement.GetUiElement<FrameworkElement>().Height = 100;
            foreach (var connector in item.Connectors)
            {
                connector.UIElement.GetUiElement<FrameworkElement>().MouseEnter += (s, e) =>
                {
                    connector.MouseIn();
                };
                item.UIElement.GetUiElement<FrameworkElement>().MouseLeave += (s, e) =>
                {
                    connector.MouseOut();
                };

                if (connector.Type == ConnectorType.Out)
                {
                    connector.UIElement.GetUiElement<FrameworkElement>().MouseLeftButtonDown += (s, e) =>
                    {
                        e.Handled = true;
                        connector.Point = e.GetPosition(editorCanvas).ToWorkFlowPoint();

                        MovingLine = CreateLine(connector, null);
                        connector.Lines.Add(MovingLine);
                        editorCanvas.Children.Add(MovingLine.UIElement.GetUiElement<FrameworkElement>());
                        SetResetConnectorCanConnect(MovingLine, WorkFlowItems);
                    };

                    connector.UIElement.GetUiElement<FrameworkElement>().MouseLeftButtonUp += (s, e) =>
                    {
                        ConnectionCreateCleanup();
                    };
                }
                else
                {
                    connector.UIElement.GetUiElement<FrameworkElement>().MouseLeftButtonUp += (s, e) =>
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

                            connector.Point = e.GetPosition(editorCanvas).ToWorkFlowPoint();
                            MovingLine.End = connector;
                            MovingLine.DrawPath(MovingLine.Start.Point, MovingLine.End.Point);
                            connector.Lines.Add(MovingLine);

                            MovingLine = new LineItem();
                            MovingLine = null;
                            SetResetConnectorCanConnect(null, WorkFlowItems);

                        }
                    };
                }
            }


            item.UIElement.GetUiElement<FrameworkElement>().MouseLeftButtonDown += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.CaptureMouse();
            };

            item.UIElement.GetUiElement<FrameworkElement>().MouseLeftButtonUp += (s, e) =>
            {
                e.Handled = true;
                var sender = s as FrameworkElement;
                sender.ReleaseMouseCapture();
                ConnectionCreateCleanup();
            };

            item.UIElement.GetUiElement<FrameworkElement>().MouseMove += (s, e) =>
            {
                editorCanvas.Width = editorCanvas.ActualWidth;
                editorCanvas.Height = editorCanvas.ActualHeight;
                scroller.Width = scroller.ActualWidth;
                scroller.Height = scroller.ActualHeight;

                e.Handled = true;
                var sender = s as FrameworkElement;
                if (sender.IsMouseCaptured)
                {
                    var point = e.GetPosition(editorCanvas);

                    if (point.X >= editorCanvas.Width - ItemWidth)
                    {
                        editorCanvas.Width += 50; scroller.ScrollToHorizontalOffset(editorCanvas.Width);
                    }
                    if (point.Y >= editorCanvas.Height - ItemHeight) { editorCanvas.Height += 50; scroller.ScrollToVerticalOffset(editorCanvas.Height); }


                    if (point.X >= scroller.ActualWidth + scroller.HorizontalOffset - ItemWidth)
                    {
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 50);
                    }
                    if (point.Y >= scroller.VerticalOffset + scroller.ActualHeight - ItemHeight) { scroller.ScrollToVerticalOffset(scroller.VerticalOffset + 50); }


                    if (point.X < scroller.HorizontalOffset + ItemWidth / 2)
                    {
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 20);
                    }
                    if (point.Y < scroller.VerticalOffset + ItemHeight / 2) { scroller.ScrollToVerticalOffset(scroller.VerticalOffset - 20); }



                    item.Move(point.ToWorkFlowPoint());
                    MakeCanvasFit((int)this.ActualWidth, (int)this.ActualHeight);
                }
            };
            return item;

        }
        public ILine CreateLine(IConnector start, IConnector end)
        {
            var line = new LineItem

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
            this.editorCanvas.Children.Remove(e.UIElement.GetUiElement<FrameworkElement>());
        }

        private void MakeCanvasFit(int minX, int minY)
        {
            var xPadding = 300;
            var yPadding = 200;
            var maxX = this.WorkFlowItems.Max(z => z.Position.X);
            var maxY = this.WorkFlowItems.Max(z => z.Position.Y);
            if (maxX + xPadding > minX) editorCanvas.Width = maxX + xPadding;
            if (maxY + yPadding > minY) editorCanvas.Height = maxY + yPadding;
        }


        private bool IsLoop(IConnector start, IConnector end)
        {
            foreach (var item in end.WorkFlowItem.Connectors.Where(z => z.Type == ConnectorType.Out))
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
