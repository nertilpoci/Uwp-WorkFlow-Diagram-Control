using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Workflow.Common.Enums;
using Workflow.Common.Models;

namespace Workflow.Common.Interface
{
    public interface IConnector 
    {
        ConnectorType Type { get; set; }
        string Label { get; set; }
        void MouseIn();
        void MouseOut();
        void SetCanConnectUi(bool reset = false);
        bool CanConnect(ILine line);
        IList<ILine> Lines { get; set; }
        IUIElement UIElement { get; set; }
        IWorkFlowItem WorkFlowItem { get; set; }
        WorkFlowPoint Point { get; set; }
        ILine AddLine(ILine line, ConnectorType type);
    }
}
