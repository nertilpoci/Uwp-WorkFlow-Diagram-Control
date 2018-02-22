using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflow.Common.Interface;

namespace WorkFlow.Wpf.ViewModels
{
    public class WorkFlowItemContentViewModel :  IWorkFlowItemContentContext
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value;  } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }
        public WorkFlowItemContentViewModel() { }
        public WorkFlowItemContentViewModel(string title, string description) { Title = title; Description = description; }
    }
}
