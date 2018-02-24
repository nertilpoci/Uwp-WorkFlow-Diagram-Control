using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;

namespace Workflow.Common.Extensions
{
   public static class IWorkFlowItemExtensions
    {
     
        public static IConnector AddWorkFlowConnector(this IWorkFlowItem item,IConnector connector)
        {
            item.Connectors.Add(connector);
            item.ItemContent.AddConnector(connector);
            return connector;

        }
        public  static void MoveItem(this IWorkFlowItem item, WorkFlowPoint point, double width, double height,Func<IConnector,WorkFlowPoint> getPosition)
        {
            foreach (var connector in item.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {

                    point.X += width / 2;
                    point.Y += height / 2;
                    if (connector.Type == ConnectorType.In)
                    {
                        line.End.Point = getPosition(connector);
                        line.DrawPath(line.Start.Point, line.End.Point);
                    }
                    else
                    {
                        line.Start.Point = getPosition(connector);
                        line.DrawPath(line.Start.Point, line.End.Point);
                    }

                }
            }
        }


      
        public static void CallNextItem(this IWorkFlowItem item, object input)
        {
           item.Connectors.Where(z => z.Type == ConnectorType.Out).ToList().ForEach(connector => {
               Parallel.ForEach(connector.Lines, line => {
                   if (line.End.WorkFlowItem is IExecutableNode) ((IExecutableNode)line.End.WorkFlowItem).Run(input);
               });
           });
        }

        public static async Task RunNode(this IExecutableNode item, object input)
        {   
            var result = await Task.Run(() => item.OnExecuteAction(input));
            Task.Run(() => item.CallNextItem(result)); // fire and forget
        }

    }
}
