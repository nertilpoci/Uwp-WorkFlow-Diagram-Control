using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;

namespace Workflow.Common.Implementation
{
    public class ConnectorBase : IConnector
    {

        protected string normalColor = "#005b96";
        protected string mouseOverColor = "#03396c";
        protected string canConnectColor = "#83AA30";
        private object currentUiElement;

        public ConnectorType Type { get; set; }
        public string Label { get; set; }
        public IList<ILine> Lines { get; set; } = new List<ILine>();
        public IWorkFlowItem WorkFlowItem { get; set; }
        public WorkFlowPoint Point { get; set; }
        public IUIElement UIElement { get; set; }

        public virtual ILine AddLine(ILine line, ConnectorType type)
        {
            Lines.Add(line);
            if (type == ConnectorType.In) line.End = this;
            else if (type == ConnectorType.Out) line.Start = this;
            return line;

        }

        public virtual bool CanConnect(ILine line)
        {
            return line != null && !this.Lines.Any(z => z.Start == line.Start) && this.Type == ConnectorType.In && line.Start.WorkFlowItem != this.WorkFlowItem;
        }

        public virtual void MouseIn()
        {
           
        }

        public virtual void MouseOut()
        {
           
        }

        public virtual void SetCanConnectUi(bool reset = false)
        {
           
        }
    }
}
