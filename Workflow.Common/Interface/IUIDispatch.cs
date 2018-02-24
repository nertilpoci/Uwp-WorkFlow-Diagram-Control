using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Common.Interface
{
    public interface IUIDispatch
    {
        void DispatchToUI(Action action);
    }
}
