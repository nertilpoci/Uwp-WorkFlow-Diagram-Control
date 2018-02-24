using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Interface;
using Workflow.Common.Models;

namespace Workflow.Common.Implementation
{
  public  class BaseLine:ILine
    {
        protected string normalStroke = "#4D648D";
        protected string mouseOverStroke = "#005b96";
      

        public event EventHandler<ILine> LineDeleted;
        public void OnLineDeleted(ILine e)
        {
            LineDeleted?.Invoke(this, e);
        }

        public string Label { get; set; }
        public virtual void MouseIn() { throw new  NotImplementedException(); }
        public virtual void MouseOut() { throw new NotImplementedException(); }
        public IConnector Start { get; set; }
        public IConnector End { get; set; }


        public IUIElement UIElement { get; set; }

        public virtual void DrawPath(WorkFlowPoint source, WorkFlowPoint destination, float magic = 8)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete()
        {
            var start = this.Start;
            var end = this.End;
            start.Lines.Remove(this);
            end.Lines.Remove(this);
            OnLineDeleted(this);
        }
    }
}
