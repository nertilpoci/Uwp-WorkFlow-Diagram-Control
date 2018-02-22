using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;
using WorkFlow.Wpf.Extensions;

namespace WorkFlow.Controls.Workflow
{
    public class Line : ILine
    {
        private string normalStroke = "#4D648D";
        private string mouseOverStroke = "#005b96";

        public Line()
        {

            Element = new Path
            {
                Stroke = new SolidColorBrush(normalStroke.HexToColor()),
                StrokeThickness = 5,
                StrokeStartLineCap = PenLineCap.Square,
                StrokeEndLineCap = PenLineCap.Square
            };

        }

        public event EventHandler<ILine> LineDeleted;
        private void OnLineDeleted(ILine e)
        {
            LineDeleted?.Invoke(this.Element, e);
        }

        private void SetContextMenu()
        {
          
        }

       

        public string Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public void MouseIn() { ((Path)Element).Stroke = new SolidColorBrush(normalStroke.HexToColor()); }
        public void MouseOut() { ((Path)Element).Stroke = new SolidColorBrush(mouseOverStroke.HexToColor()); }
        public IConnector Start { get; set; }
        public IConnector End { get; set; }

        public object Element {get; }

        public void DrawPath(WorkFlowPoint source, WorkFlowPoint destination,float magic=8)
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
            ((Path)Element).Data = geo;
            Canvas.SetZIndex(((Path)Element), -2);
        }

        public void Delete()
        {
            var start = this.Start;
            var end = this.End;
            start.Lines.Remove(this);
            end.Lines.Remove(this);
            OnLineDeleted(this);
        }

        
    }
}
