using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Workflow.Common.Implementation;
using WorkFlow.Wpf.Helpers;

namespace WorkFlow.Wpf.Impl
{
   public class WpfExecutableNodeBase:ExecutableNodeBase
    {
        public WpfExecutableNodeBase(FrameworkElement parent):base(parent)
        {

        }
        public override void DispatchToUI(Action action)
        {
            DispatchService.Invoke(action);
        }
    }
}
