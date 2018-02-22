using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;
using WorkFlow.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class ConnectorControl : UserControl, IConnector
    {
        private string normalColor = "#005b96";
        private string mouseOverColor = "#03396c";
        private string canConnectColor = "#83AA30";
        public ConnectorControl()
        {
            this.InitializeComponent();
            //anchor.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public ConnectorType Type { get; set; }
        public string Label { get; set; }


        public IList<ILine> Lines { get; set; } = new List<ILine>();
        public IWorkFlowItem WorkFlowItem { get; set; }
        public WorkFlowPoint Point { get; set; }
        public object Element { get => this; }

        public ILine AddLine(ILine line,ConnectorType type)
        {
            Lines.Add(line);
            if (type == ConnectorType.In) line.End = this;
            else if (type == ConnectorType.Out) line.Start = this;
            return line;
            
        }

        public bool CanConnect(ILine line)
        {
            return line!=null && !this.Lines.Any(z=>z.Start==line.Start) && this.Type == ConnectorType.In && line.Start.WorkFlowItem != this.WorkFlowItem;
        }

        public void MouseIn()
        {
            anchor.Fill = new SolidColorBrush(mouseOverColor.HexToColor());
        }

        public void MouseOut()
        {
            anchor.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public void SetCanConnectUi(bool reset = false)
        {
           anchor.Fill=reset? new SolidColorBrush(normalColor.HexToColor()) :new SolidColorBrush(canConnectColor.HexToColor());
        }
    }
}
