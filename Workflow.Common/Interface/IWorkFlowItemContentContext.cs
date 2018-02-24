using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Common.Interface
{
  public  interface IWorkFlowItemContentContext
    {
        string Title { get; set; }
        string Description { get; set; }
    }
}
