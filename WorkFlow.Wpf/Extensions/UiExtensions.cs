using System.Windows;

namespace WorkFlow.Wpf.Extensions
{
    public static  class UIExtensions
    {

        public static FrameworkElement ToFrameworkElement(this object frameworkobject)
        {
            return (FrameworkElement)frameworkobject;
        }
      
    }
}
