using GalaSoft.MvvmLight;
using Workflow.Common.Interface;

namespace WorkFlow.ViewModels
{
    public class WorkFlowItemContentViewModel:ViewModelBase, IWorkFlowItemContentContext
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged(); } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged(); } }
        public WorkFlowItemContentViewModel() { }
        public WorkFlowItemContentViewModel(string title,string description) { Title = title; Description = description; }
    }
}
