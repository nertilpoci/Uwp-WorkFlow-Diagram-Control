using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Workflow.Common.Models;

namespace WorkFlow.Extensions
{
  public static  class UIExtensions
    {

        public static FrameworkElement ToFrameworkElement(this object frameworkobject)
        {
            return (FrameworkElement)frameworkobject;
        }
      
    }
}
