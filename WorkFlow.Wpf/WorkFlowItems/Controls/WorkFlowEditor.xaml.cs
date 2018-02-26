using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;
using WorkFlow.Wpf.Extensions;
using WorkFlow.Wpf.WorkFlowItems.Items;
using ZoomAndPan;

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
               

                e.Handled = true;
                var sender = s as FrameworkElement;
                if (sender.IsMouseCaptured)
                {
                    var point = e.GetPosition(editorCanvas);

                    if (point.X >= editorCanvas.Width - ItemWidth +10)
                    {
                        editorCanvas.Width += 5;
                        zoomAndPanControl.ContentOffsetX += 5;
                    }
                    if (point.Y >= editorCanvas.Height - ItemWidth +10)
                    {
                        editorCanvas.Height += 5;
                        zoomAndPanControl.ContentOffsetY +=5;
                    }

                    if (point.X <= 100) return;
                    if (point.Y <=50) return;


                    item.Move(point.ToWorkFlowPoint());
                   
                    MakeCanvasFit((int)this.ActualWidth, (int)this.ActualHeight);
                }
            };
            return item;

        }
        public WorkFlowPoint GetPosition()
        {
            var transform = this.TransformToVisual(zoomAndPanControl);
            return transform.Transform(new Point(0, 0)).ToWorkFlowPoint();

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
            var offsetXToRemove = zoomAndPanControl.ActualWidth + zoomAndPanControl.ContentOffsetX - editorCanvas.Width;
            var offsetYToRemove = zoomAndPanControl.ActualHeight + zoomAndPanControl.ContentOffsetY - editorCanvas.Height;

            if(offsetXToRemove>0) zoomAndPanControl.ContentOffsetX -= offsetXToRemove;
            if(offsetYToRemove > 0) zoomAndPanControl.ContentOffsetY -= offsetYToRemove;


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



        /// zoom pan
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;
        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            editorCanvas.Focus();
            Keyboard.Focus(editorCanvas);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(editorCanvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomAndPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn();
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut();
                    }
                }

                zoomAndPanControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(editorCanvas);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;
               
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        private void ZoomOut()
        {

          
           
            var maxX = this.WorkFlowItems.Max(z => z.Position.X);
            var maxY = this.WorkFlowItems.Max(z => z.Position.Y);
            var xFits = zoomAndPanControl.ContentViewportWidth > maxX;
            var yFits = zoomAndPanControl.ContentViewportHeight > maxY;
            if (!xFits && !yFits) zoomAndPanControl.ContentScale -= 0.1;
            else if (!xFits)
            {
                if (editorCanvas.ActualHeight < this.ActualHeight) editorCanvas.Height = this.ActualHeight;
                zoomAndPanControl.ContentScale -= 0.1;
            }
            else if (!yFits)
            {
                if (editorCanvas.ActualWidth < this.ActualWidth) editorCanvas.Width = this.ActualWidth;
                zoomAndPanControl.ContentScale -= 0.1;
            }

        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        private void ZoomIn()
        {
            zoomAndPanControl.ContentScale += 0.1;
        }

        /// <summary>

        //end of zoom pan
    }
}
