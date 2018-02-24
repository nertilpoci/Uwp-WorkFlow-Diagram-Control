using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Common.Interface
{
    public interface ITriggerNode
    {
        Task Start(params object[] args);
    }
}
