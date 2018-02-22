using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Workflow.Common.Models;

namespace WorkFlow.Extensions
{
  public static  class PositioningExtensions
    {

        public static Point CreateWindowsFoundationPoint(this WorkFlowPoint point)
        {
            return new Point(point.X, point.Y);
        }
        public static WorkFlowPoint CreateWorkFlowPoint(this Point point)
        {
            return new WorkFlowPoint(point.X, point.Y);
        }
    }
}
