using Windows.UI.Core;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class ServiceTogglerViewModel<TS> : ViewModelBase, IServiceTogglerViewModel where TS : struct 
    {
        private bool _isEnabled;
        private readonly StatefulPipelineService<TS> _service;

        public ServiceTogglerViewModel(StatefulPipelineService<TS> service, CoreDispatcher dispatcher) : base(dispatcher)
        {
            _service = service;
            _isEnabled = !service.State.Equals(default(TS));
            _service.StateChanged += HandleStateChanged;
            _service.ErrorOccured += HandleErrorOccured;
        }

        private async void HandleErrorOccured(object sender, ServiceErrorEventArgs e)
        {
            if (sender != _service) return;
            _isEnabled = !_service.State.Equals(default(TS));
            await Dispatch(() => RaisePropertyChanged(nameof(IsEnabled)));
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;

                if (value)
                {
                    var _ = _service.Start();
                }
                else
                {
                    var _ = _service.Stop();
                }
            }
        }

        private async void HandleStateChanged(object sender, ServiceStateChangedEventArgs<TS> e)
        {
            _isEnabled = !e.State.Equals(default(TS));
            await Dispatch(() => RaisePropertyChanged(nameof(IsEnabled)));
        }
    }
}