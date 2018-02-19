using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkFlow.Interface;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class WorkFlowItem : UserControl, IWorkFlowItem
    {
        UIElement parent;
        public WorkFlowItem(UIElement parent)
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.parent = parent;
        }
        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        public string Title { get; set; }
        public string Description { get; set; }

        public FrameworkElement Element => this;

        public Point Position { get { return GetPosition(); } set { } }


        private Point GetPosition()
        {
            return Element.TransformToVisual(parent).TransformPoint(new Point(0, 0)); ;
        }

        
        public void ConstructControl()
        {
            var inputConnector = new ConnectorControl { Type = ConnectorType.In, Label="In", Height=20, Width=20 };
            var inputConnector2 = new ConnectorControl { Type = ConnectorType.In, Label="In", Height=20, Width=20 };
            var outputConnector = new ConnectorControl { Type = ConnectorType.Out, Label="Out", Height = 20, Width = 20 };

            Connectors.Add(inputConnector);
            Connectors.Add(outputConnector);
            Connectors.Add(inputConnector2);

            inputConnectors.Children.Add(inputConnector);
            inputConnectors.Children.Add(inputConnector2);
            outputConnectors.Children.Add(outputConnector);
        }
    }
}
