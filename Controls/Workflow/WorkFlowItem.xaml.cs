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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WorkFlow.Controls.Workflow
{
    public sealed partial class WorkFlowItem : UserControl, IWorkFlowItem, INotifyPropertyChanged
    {
        UIElement parent;
        public WorkFlowItem(UIElement parent)
        {
            this.InitializeComponent();
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

        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        private string _title;
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(); } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged(); } }

        public FrameworkElement Element => this;
        private float magic = 8;
        public void Move( Point point)
        {
            Canvas.SetLeft(this, point.X - this.ActualWidth / 2);
            Canvas.SetTop(this.Element, point.Y - this.ActualHeight / 2);

            foreach (var connector in this.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var path = line as Line;
                    var ui = connector as UserControl;
                    var transform = ui.TransformToVisual(parent);
                    Point absolutePosition = transform.TransformPoint(new Point(0, 0));
                    absolutePosition.X += ui.ActualWidth / 2;
                    absolutePosition.Y += ui.ActualHeight / 2;
                    if (connector.Type == Interface.ConnectorType.In)
                    {
                        line.End.Point = absolutePosition;
                        path.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }
                    else
                    {
                        line.Start.Point = absolutePosition;
                        path.DrawPath(line.Start.Point, line.End.Point,this.magic);
                    }

                }
            }
        }
        private Point _position;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Point Position { get { return GetPosition(); } set { _position = value; } }

        public InputOutputConnectorPosition ConnectorLayout { get; set; } = InputOutputConnectorPosition.RightLeft;

        private Point GetPosition()
        {
            return Element.TransformToVisual(parent).TransformPoint(new Point(0, 0)); ;
        }

        public void ConstructControl()
        {
            var inputConnector = new ConnectorControl { Type = ConnectorType.In, Label="In", Height=20, Width=20 };
            var inputConnector2 = new ConnectorControl { Type = ConnectorType.In, Label="In", Height=20, Width=20 };
            var outputConnector = new ConnectorControl { Type = ConnectorType.Out, Label="Out", Height = 20, Width = 20 };

            AddConnector(inputConnector);
            AddConnector(outputConnector);
            AddConnector(inputConnector2);

            inputConnectors.Children.Add(inputConnector);
            inputConnectors.Children.Add(inputConnector2);
            outputConnectors.Children.Add(outputConnector);
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
                    this.magic = -8;

                    break;
                case InputOutputConnectorPosition.BottomTop:
                    ChangeInputLayout(Orientation.Horizontal, Dock.Bottom, Dock.Top);
                    break;
                default:
                    break;  
            }
            Move(GetPosition());
        }
    }
}
