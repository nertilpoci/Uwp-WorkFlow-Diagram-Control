using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Interface;

namespace WorkFlow.ViewModels
{
   public class WorkFlowItemViewModel:ViewModelBase, IWorkFlowItemContent
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged(); } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged(); } }
        public WorkFlowItemViewModel() { }
        public WorkFlowItemViewModel(string title,string description) { Title = title; Description = description; }
    }
}
