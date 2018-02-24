using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Models;

namespace Workflow.Common.Interface
{
    public interface ILine
    {

        string Label { get; set; }
        IConnector Start { get; set; }
        IConnector End { get; set; }
        void MouseIn();
        void MouseOut();
        IUIElement UIElement { get; set; }
        void DrawPath(WorkFlowPoint source, WorkFlowPoint destination, float magic = 8);
        void Delete();
        event EventHandler<ILine> LineDeleted;
    }
}
