using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkFlow.Enums;
using WorkFlow.Interface;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WorkFlow.Impl;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class WorkFlowItem : ExecutableNodeBase, IWorkFlowItem, INotifyPropertyChanged, IExecutableNode
    {
        FrameworkElement parent;
        public WorkFlowItem(FrameworkElement parent):base(parent)
        {
            this.InitializeComponent();
            base.Element = this;
            this.DataContext = this;
            this.parent = parent;
            this.RightTapped += WorkFlowItem_RightTapped;
            ChangeConnectorLayoutCommand = new RelayCommand<InputOutputConnectorPosition>(ChangeOrientation);
        }

        public ICommand ChangeConnectorLayoutCommand { get; set; }

        private void WorkFlowItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            
            //this.magic += Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down)?  -1:1;
            //Move(GetPosition());
        }


        public void Move( Point point)
        {
            base.Move(point, this.ActualWidth, this.ActualHeight);
        }
      

      

        public void ConstructControl(IConnector[] connectors)
        {

            foreach (var connector in connectors)
            {
            AddConnector(connector);
                if (connector.Type == ConnectorType.In) inputConnectors.Children.Add(connector.Element);
                else if (connector.Type == ConnectorType.Out) outputConnectors.Children.Add(connector.Element);
            }
           

            
        }

        public IConnector AddConnector(IConnector connector)
        {
                Connectors.Add(connector);
                connector.WorkFlowItem = this;
                return connector;
        }

   
        private void ChangeInputLayout(Windows.UI.Xaml.Controls.Orientation orientation, Dock inputDock, Dock outputDock )
        {
            foreach (var item in outputConnectors.Children)
            {
                ((FrameworkElement)item).Margin = orientation== Orientation.Vertical? new Thickness(0, 5, 0, 5):new Thickness(5, 0, 5, 0);
            }
            foreach (var item in inputConnectors.Children)
            {
                ((FrameworkElement)item).Margin = orientation == Orientation.Vertical ?  new Thickness(0, 5, 0, 5): new Thickness(5, 0, 5, 0);
            }
            itemInfo.Margin = orientation == Orientation.Vertical ? new Thickness(-10, 0, -10, 0) :  new Thickness(0, -10, 0, -10);

            outputConnectors.Orientation = orientation;
            inputConnectors.Orientation = orientation;
          
            DockPanel.SetDock(inputConnectors, inputDock);
            DockPanel.SetDock(outputConnectors, outputDock);
        }
        public void ChangeOrientation(InputOutputConnectorPosition layout)
        {
            switch (layout)
            {
                case InputOutputConnectorPosition.LeftRight:
                    ChangeInputLayout(Orientation.Vertical, Dock.Right, Dock.Left);
                    break;
                case InputOutputConnectorPosition.TopBottom:
                    ChangeInputLayout(Orientation.Horizontal, Dock.Top, Dock.Bottom);
                    break;
                case InputOutputConnectorPosition.RightLeft:
                    ChangeInputLayout(Orientation.Vertical, Dock.Left, Dock.Right);

                    break;
                case InputOutputConnectorPosition.BottomTop:
                    ChangeInputLayout(Orientation.Horizontal, Dock.Bottom, Dock.Top);
                    break;
                default:
                    break;  
            }
            Move(GetPosition());
        }

        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        public void Run(params object[] args)
        {
            Run(Connectors, args);
        }
    }
}
