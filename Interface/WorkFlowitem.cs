using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace WorkFlow.Interface
{
   public interface IWorkFlowItem:IUIElement
    {
     IList<IConnector> Connectors { get; set; }
     string Title { get; set; }
     string Description { get; set; }
     
     Point Position { get; set; }
     void ConstructControl();
    }
    public interface IUIElement
    {
        FrameworkElement Element { get;  }
    }
    public interface IConnector: IUIElement
    {
        ConnectorType Type { get; set; }
        string Label { get; set; }
        void MouseIn();
        void MouseOut();
        
        void SetCanConnectUi(bool reset = false);
        
        bool CanConnect(ILine line);
        IList<ILine> Lines { get; set; }
        IWorkFlowItem WorkFlowItem { get; set; }
        Point Point { get; set; }
    }
    public interface ILine: IUIElement
    {
        string Label { get; set; }
        Action MouseIn { get; set; }
        Action MouseOut { get; set; }
        IConnector Start { get; set; } 
        IConnector End { get; set; }
    }
    public enum ConnectorType
    {
        In,
        Out
    }
}
