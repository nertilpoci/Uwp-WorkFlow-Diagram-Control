using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WorkFlow.Extensions;
using WorkFlow.Interface;

namespace WorkFlow.Controls.Workflow
{
    public class Line : Path, ILine
    {
        public Line()
        {

            //Stroke = new LinearGradientBrush()
            //{
            //    EndPoint = new Point(0.5, 1),
            //    StartPoint = new Point(0.5, 0),
            //    GradientStops = new GradientStopCollection() {
            //            new GradientStop { Color= "#4D648D".HexToColor() },

            //            new GradientStop { Color= "#F1F1F2".HexToColor(),Offset=0.23 },
            //            new GradientStop { Color= "#4D648D".HexToColor(),Offset=0.80 }
            //}
            //};
            Stroke = new SolidColorBrush("#4D648D".HexToColor());
            StrokeThickness = 5;
            StrokeStartLineCap = PenLineCap.Square;
            StrokeEndLineCap = PenLineCap.Square;
               
            
        }
        public string Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action MouseIn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action MouseOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IConnector Start { get; set; }
        public IConnector End { get; set; }

        public FrameworkElement Element => this;

        public void DrawPath(Point source, Point destination,float magic=8)
        {
            
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
            this.Data = geo;
            Canvas.SetZIndex(this, -2);
        }
    }
}
