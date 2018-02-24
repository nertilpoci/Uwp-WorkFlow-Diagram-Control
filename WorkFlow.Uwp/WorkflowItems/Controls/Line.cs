using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;

namespace WorkFlow.WorkFlowItems.Controls
{
    public class Line : Path, IUIElement
    {
        protected string normalStroke = "#4D648D";
        protected string mouseOverStroke = "#005b96";
        public Line()
        {

           
            Stroke = new SolidColorBrush(normalStroke.HexToColor());
            StrokeThickness = 5;
            StrokeStartLineCap = PenLineCap.Square;
            StrokeEndLineCap = PenLineCap.Square;
            
            
        }

        public T GetUiElement<T>() where T : class
        {
            return this as T;
        }

        public double ItemWidth => throw new NotImplementedException();

        public double ItemHeight => throw new NotImplementedException();

        public WorkFlowPoint GetPosition()
        {
            throw new NotImplementedException();
        }

        public void SetPosition(WorkFlowPoint point)
        {
            throw new NotImplementedException();
        }
    }
}
