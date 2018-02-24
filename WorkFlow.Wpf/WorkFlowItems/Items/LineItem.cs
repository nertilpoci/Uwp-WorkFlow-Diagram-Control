using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Workflow.Common.Implementation;
using Workflow.Common.Models;
using WorkFlow.Extensions;
using WorkFlow.Wpf.WorkFlowItems.Controls;

namespace WorkFlow.Wpf.WorkFlowItems.Items
{
    public class LineItem : BaseLine
    {
        private Path _currentUIElement;

        public LineItem()
        {
            _currentUIElement = new Path();
           _currentUIElement.Stroke = new SolidColorBrush(normalStroke.HexToColor());
           _currentUIElement.StrokeThickness = 5;
           _currentUIElement.StrokeStartLineCap = PenLineCap.Square;
            _currentUIElement.StrokeEndLineCap = PenLineCap.Square;
             UIElement = new LineControl(_currentUIElement);
        }
        public override void MouseIn()
        {
            _currentUIElement.Stroke = new SolidColorBrush(mouseOverStroke.HexToColor());
        }

        public override void MouseOut()
        {
            _currentUIElement.Stroke = new SolidColorBrush(normalStroke.HexToColor());
        }
        public override void DrawPath(WorkFlowPoint source, WorkFlowPoint destination, float magic = 8)
        {
             var segments = new List<PathSegment>();
            if (destination.X > source.X)
            {
                WorkFlowPoint c1 = new WorkFlowPoint(source.X + (magic - 1) * (destination.X - source.X) / magic, source.Y + (destination.Y - source.Y) / magic);
                WorkFlowPoint c2 = new WorkFlowPoint(source.X + (destination.X - source.X) / magic, source.Y + (magic - 1) * (destination.Y - source.Y) / magic);
                var bs = new BezierSegment();
                bs.Point1 = c1.CreateWindowsFoundationPoint();
                bs.Point2 = c2.CreateWindowsFoundationPoint();
                bs.Point3 = destination.CreateWindowsFoundationPoint();
                segments.Add(bs);
            }
            else
            {
                double distanceX = Math.Abs(source.X - destination.X) / 3;
                WorkFlowPoint c1 = new WorkFlowPoint(source.X + Math.Min((magic - 1) * (source.X - destination.X) / magic, distanceX) + 20, source.Y + (magic - 1) * (destination.Y - source.Y) / magic);
                WorkFlowPoint c2 = new WorkFlowPoint(destination.X - Math.Min((magic - 1) * (source.X - destination.X) / magic, distanceX) - 20, destination.Y - (magic - 1) * (destination.Y - source.Y) / magic);
                var bs = new BezierSegment();
                bs.Point1 = c1.CreateWindowsFoundationPoint();
                bs.Point2 = c2.CreateWindowsFoundationPoint();
                bs.Point3 = destination.CreateWindowsFoundationPoint();
                segments.Add(bs);
            }

            var geo = new PathGeometry();
            var pf = new PathFigure() { StartPoint = source.CreateWindowsFoundationPoint(), IsClosed = false };
            foreach (var s in segments)
            {
                pf.Segments.Add(s);
            }
            geo.Figures.Add(pf);
            _currentUIElement.Data = geo;
            Canvas.SetZIndex(_currentUIElement, -2);
        }
    }
}
