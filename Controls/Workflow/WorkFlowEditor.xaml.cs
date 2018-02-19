
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WorkFlow.Extensions;
using WorkFlow.Interface;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class WorkFlowEditor : UserControl
    {
        int ItemHeight = 100;
        int ItemWidth = 200;
        private IList<IWorkFlowItem> WorkFlowItems = new List<IWorkFlowItem>();
        public WorkFlowEditor()
        {
            InitializeComponent();

           
            SetScrollViewerEvents();
            SetCanvasEvents();
            AddTestControls();
        }
        private void AddTestControls()
        {
            var item1 = CreateNew();
            var item2 = CreateNew();

            WorkFlowItems.Add(item1);
            WorkFlowItems.Add(item2);
            editorCanvas.Children.Add(item1.Element);
            editorCanvas.Children.Add(item2.Element);
            Canvas.SetLeft(item1.Element, 200);
            Canvas.SetTop(item2.Element, 200);

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
        private void SetCanvasEvents()
        {
            editorCanvas.IsRightTapEnabled = true;
            editorCanvas.RightTapped += (s, e) => {
               var position= e.GetPosition(editorCanvas);
                var item = CreateNew();
                Canvas.SetLeft(item.Element, position.X);
                Canvas.SetTop(item.Element, position.Y);
                editorCanvas.Children.Add(item.Element);
                WorkFlowItems.Add(item);

            };

            editorCanvas.PointerReleased += (s, e) =>
            {
                if (MovingLine == null) return;
                this.editorCanvas.Children.Remove(MovingLine.Element);
                MovingLine = null;
            };

            editorCanvas.PointerMoved += (s, e) => {
                if (MovingLine == null) return;
                DrawPath(MovingLine.Start.Point, e.GetCurrentPoint(editorCanvas).Position, (Line)MovingLine);
            };

        }
        ILine MovingLine;
        private IWorkFlowItem CreateNew()
        {
            IWorkFlowItem item = new WorkFlowItem(editorCanvas);
            item.Element.RenderTransform = new TranslateTransform();
            item.ConstructControl();

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

                        MovingLine = ControlHelper.CreateLine(connector, null);
                        connector.Lines.Add(MovingLine);
                        editorCanvas.Children.Add(MovingLine.Element);
                        foreach (var wE in WorkFlowItems)
                        {
                            foreach (var c in wE.Connectors)
                            {
                                c.SetCanConnectUi(c.CanConnect(MovingLine) == false);
                            }
                        }
                    };

                    connector.Element.PointerReleased += (s, e) =>
                    {

                        if (MovingLine == null) return;
                        this.editorCanvas.Children.Remove(MovingLine.Element);
                        MovingLine.Start.Lines.Remove(MovingLine);
                        MovingLine = null;

                    };
                }
                else
                {
                    connector.Element.PointerReleased += (s, e) =>
                    {

                        if (MovingLine != null)
                        {

                            e.Handled = true;

                            connector.Point = e.GetCurrentPoint(editorCanvas).Position;
                            MovingLine.End = connector;

                            DrawPath(MovingLine.Start.Point, MovingLine.End.Point, (Line)MovingLine.Element);
                            connector.Lines.Add(MovingLine);

                            MovingLine = new Line();
                            MovingLine = null;
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



                    MoveWorkFlowItem(item, point);
                    MakeCanvasFit((int)this.ActualWidth, (int)this.ActualHeight);
                }
            };


            return item;
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
        private void MoveWorkFlowItem(IWorkFlowItem item, Point point)
        {
            Canvas.SetLeft(item.Element, point.X - item.Element.ActualWidth / 2);
            Canvas.SetTop(item.Element, point.Y - item.Element.ActualHeight / 2);

            foreach (var connector in item.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var path = line as Line;
                    var ui = connector as UserControl;
                    var transform = ui.TransformToVisual(editorCanvas);
                    Point absolutePosition = transform.TransformPoint(new Point(0, 0));
                    absolutePosition.X += ui.ActualWidth / 2;
                    absolutePosition.Y += ui.ActualHeight / 2;
                    if (connector.Type == Interface.ConnectorType.In)
                    {
                        line.End.Point = absolutePosition;
                        DrawPath(line.Start.Point, line.End.Point, path);
                    }
                    else
                    {
                        line.Start.Point = absolutePosition;
                        DrawPath(line.Start.Point, line.End.Point, path);
                    }

                }
            }
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

    }
    public static class ControlHelper
    {

        public static ILine CreateLine(IConnector start, IConnector end)
        {
            var line = new Line

            {
                Start = start,
                End = end
            };

            return line;
        }
    }
}


