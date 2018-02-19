using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Models;

namespace WorkFlow.Interface
{
    public interface IWorkFlowEditor
    {
        bool AllowLoops { get; set; }
        event EventHandler<WorkFlowLoopEventModel> LoopDetected;

    }
}
