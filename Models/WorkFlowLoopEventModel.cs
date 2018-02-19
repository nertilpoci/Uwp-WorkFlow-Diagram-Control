using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Interface;

namespace WorkFlow.Models
{
    public class WorkFlowLoopEventModel
    {
        public IConnector Start { get; set; }
        public IConnector End { get; set; }
        public WorkFlowLoopEventModel() { }
        public WorkFlowLoopEventModel(IConnector start, IConnector end) {
            Start = start;
            End = end;
        }

    }
}
