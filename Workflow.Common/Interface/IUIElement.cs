using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Models;

namespace Workflow.Common.Interface
{
    public interface IUIElement 
    {
        void SetPosition(WorkFlowPoint point);
        WorkFlowPoint GetPosition();
        double ItemWidth { get;}
        double ItemHeight { get;  }
        T GetUiElement<T>() where T : class;
    }
}
