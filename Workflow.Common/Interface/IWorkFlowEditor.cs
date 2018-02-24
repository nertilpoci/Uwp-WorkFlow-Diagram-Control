using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Models;

namespace Workflow.Common.Interface
{
    public interface IWorkFlowEditor
    {
        bool AllowLoops { get; set; }
        event EventHandler<WorkFlowLoopEventModel> LoopDetected;

    }
}
