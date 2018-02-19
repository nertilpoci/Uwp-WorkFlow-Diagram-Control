using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkFlow.Extensions;
using WorkFlow.Interface;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class ConnectorControl : UserControl, IConnector
    {
        private string normalColor = "#005b96";
        private string mouseOverColor = "#03396c";
        private string canConnectColor = "#83AA30";
        public ConnectorControl()
        {
            this.InitializeComponent();
            //anchor.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public ConnectorType Type { get; set; }
        public string Label { get; set; }


        public IList<ILine> Lines { get; set; } = new List<ILine>();
        public IWorkFlowItem WorkFlowItem { get; set; }
        public Point Point { get; set; }
        public FrameworkElement Element { get => this; }

        public bool CanConnect(ILine line)
        {
         return   this.Type == ConnectorType.In;
        }

        public void MouseIn()
        {
            anchor.Fill = new SolidColorBrush(mouseOverColor.HexToColor());
        }

        public void MouseOut()
        {
            anchor.Fill = new SolidColorBrush(normalColor.HexToColor());
        }

        public void SetCanConnectUi(bool reset = false)
        {
           anchor.Fill=reset? new SolidColorBrush(normalColor.HexToColor()) :new SolidColorBrush(canConnectColor.HexToColor());
        }
    }
}
