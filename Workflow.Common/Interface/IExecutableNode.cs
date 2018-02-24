using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Common.Interface
{
    public interface IExecutableNode:IWorkFlowItem
    {
        bool IsExecuting { get; set; }
        Func<object, Task<object>> OnExecuteAction { get; set; }
        Task Run(object input = null);
    }
}
