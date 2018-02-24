using Windows.UI.Xaml;

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
