using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Interface
{
    public interface IWorkFlowItemContentContext
    {
         string Title { get; set; }
         string Description { get; set; }
         

      
    }
    public interface IWorkFlowItemContent
    {
        void AddConnector(IConnector connector);
        IWorkFlowItemContentContext ItemContentContext { get; set; }

    }
}
