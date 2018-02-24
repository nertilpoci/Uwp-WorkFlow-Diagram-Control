

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Workflow.Common.Enums;
using Workflow.Common.Interface;
using Workflow.Common.Models;

namespace WorkFlow.Common.Implementation
{
    public abstract class BaseWorkFlowElement:IWorkFlowItem, INotifyPropertyChanged
    {
        object parent;
        public BaseWorkFlowElement(object parent)
        {
            this.parent = parent;
            
        }
        public IConnector AddConnector(IConnector connector)
        {
            Connectors.Add(connector);
            ItemContent.AddConnector(connector);
            return connector;
        }
        public IList<IConnector> Connectors { get; set; } = new List<IConnector>();
        public object Element { get; set; }
        private IWorkFlowItemContent _itemContent;
        public IWorkFlowItemContent ItemContent { get { return _itemContent; } set { _itemContent = value; OnPropertyChanged(); } }
        private float magic = 8;
        public void Move(WorkFlowPoint point)
        {

            UIElement.SetPosition(new WorkFlowPoint(point.X - UIElement.ItemWidth / 2, point.Y - UIElement.ItemHeight / 2));

            foreach (var connector in this.Connectors.Where(z => z.Lines.Any()))
            {
                foreach (var line in connector.Lines)
                {
                    var absolutePosition = connector.UIElement.GetPosition();
                    absolutePosition.X += connector.UIElement.ItemWidth / 2;
                    absolutePosition.Y += connector.UIElement.ItemHeight / 2;
                    if (connector.Type == ConnectorType.In)
                    {
                        line.End.Point = absolutePosition;
                        line.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }
                    else
                    {
                        line.Start.Point = absolutePosition;
                        line.DrawPath(line.Start.Point, line.End.Point, this.magic);
                    }


                }
            }
        }
        private WorkFlowPoint _position;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void DispatchToUI(Action action)
        {
            throw new NotImplementedException();
        }

        public WorkFlowPoint Position { get { return UIElement.GetPosition(); } set { _position = value; } }
        public IUIElement UIElement { get; set; }
       
    }
}
