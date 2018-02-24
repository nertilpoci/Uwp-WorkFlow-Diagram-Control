using System;
using System.Collections.Generic;
using Workflow.Common.Models;

namespace Workflow.Common.Interface
{
    public interface IWorkFlowItem:IUIDispatch
    {
        IList<IConnector> Connectors { get; set; }
        IUIElement UIElement { get; set; }
        IWorkFlowItemContent ItemContent { get; set; }
        WorkFlowPoint Position { get; set; }
        IConnector AddConnector(IConnector connector);
        void Move(WorkFlowPoint point);
    }
}
