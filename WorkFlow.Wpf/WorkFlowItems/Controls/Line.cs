using System;
using System.Windows.Shapes;
using Workflow.Common.Interface;
using Workflow.Common.Models;

namespace WorkFlow.Wpf.WorkFlowItems.Controls
{
    public class LineControl : IUIElement
    {
        private Path _curerntUiElement;
        public LineControl(Path control)
        {
            _curerntUiElement = control;
        }
        public T GetUiElement<T>() where T : class
        {
            return _curerntUiElement as T;
        }

        public double ItemWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double ItemHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
