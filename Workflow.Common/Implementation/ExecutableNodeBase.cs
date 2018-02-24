using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using WorkFlow.Common.Implementation;

namespace Workflow.Common.Implementation
{
    public abstract class ExecutableNodeBase : BaseWorkFlowElement, IExecutableNode
    {
        public ExecutableNodeBase(object parent) : base(parent) { }
        private bool _isExecuting;
        public bool IsExecuting { get { return _isExecuting; } set { _isExecuting = value; OnPropertyChanged(); } }
        public Func<object, Task<object>> OnExecuteAction { get; set; }
        public virtual async Task Run(object input = null)
        {
            DispatchToUI(() => { IsExecuting = true; });
            var result = await Task.Run(() => OnExecuteAction(input));
            Task.Run(() => CallNextItem(result)); // fire and forget
            DispatchToUI(() => { IsExecuting = false; });
        }
        private  void CallNextItem(object input)
        {
            Connectors.Where(z => z.Type == ConnectorType.Out).ToList().ForEach(connector => {
                Parallel.ForEach(connector.Lines, line => {
                    if (line.End.WorkFlowItem is IExecutableNode) ((IExecutableNode)line.End.WorkFlowItem).Run(input);
                });
            });

        }
    }
}
