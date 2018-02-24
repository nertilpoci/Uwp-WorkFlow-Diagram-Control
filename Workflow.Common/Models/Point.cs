using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Common.Models
{
    public class WorkFlowPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public  WorkFlowPoint(){}
        public  WorkFlowPoint(double x, double y){ X = x;Y = y; }
    }
}
