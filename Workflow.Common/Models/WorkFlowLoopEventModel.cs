using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Interface;

namespace Workflow.Common.Models
{
    public class WorkFlowLoopEventModel
    {
        public IConnector Start { get; set; }
        public IConnector End { get; set; }
        public WorkFlowLoopEventModel() { }
        public WorkFlowLoopEventModel(IConnector start, IConnector end)
        {
            Start = start;
            End = end;
        }

    }
}
