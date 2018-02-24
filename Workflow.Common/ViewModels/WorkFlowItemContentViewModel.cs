using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Common.Interface;

namespace Workflow.Common.ViewModels
{
    public class WorkFlowItemContentViewModel : IWorkFlowItemContentContext
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value; } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }
        public WorkFlowItemContentViewModel() { }
        public WorkFlowItemContentViewModel(string title, string description) { Title = title; Description = description; }
    }
}
