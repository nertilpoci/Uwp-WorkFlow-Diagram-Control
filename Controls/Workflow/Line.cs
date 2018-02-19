using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
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

    }
}
