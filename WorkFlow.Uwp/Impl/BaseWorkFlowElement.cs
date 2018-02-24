using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Workflow.Common.Implementation;

namespace WorkFlow.Impl
{
    public abstract class UWPExecutableNodeBase: ExecutableNodeBase
    {
        public UWPExecutableNodeBase(FrameworkElement parent):base(parent)
        {

        }
        public async override void DispatchToUI(Action action)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action?.Invoke());
        }

    }




}
