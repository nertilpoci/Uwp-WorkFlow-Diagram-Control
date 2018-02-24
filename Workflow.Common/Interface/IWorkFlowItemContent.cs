using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Common.Interface
{
    public interface IWorkFlowItemContent
    {
        void AddConnector(IConnector connector);
        IWorkFlowItemContentContext ItemContentContext { get; set; }

    }
}
