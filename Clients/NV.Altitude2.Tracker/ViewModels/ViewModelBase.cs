using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Annotations;

namespace NV.Altitude2.Tracker.ViewModels
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        private CoreDispatcher _dispatcher;

        protected ViewModelBase(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected async Task Dispatch(DispatchedHandler handler)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}