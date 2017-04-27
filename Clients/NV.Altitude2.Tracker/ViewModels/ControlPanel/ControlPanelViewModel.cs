using System.Threading.Tasks;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class ControlPanelViewModel : ViewModelBase
    {
        private readonly Pipeline _pipeline;
        private bool _isPipelineRunning;

        internal ControlPanelViewModel(Pipeline pipeline, ServicesCollection collection, CoreDispatcher dispatcher):base(dispatcher)
        {
            _pipeline = pipeline;
            var settings = collection.ApplicationSettings.Current;

            _isPipelineRunning = pipeline.State != GenericState.Disabled;
            pipeline.StateChanged += async (o, e) => await Dispatch(() =>
            {
                _isPipelineRunning = e.State.Equals(GenericState.Enabled);
                RaisePropertyChanged(nameof(IsPipelineRunning));
            });

            LocationService = new ServiceTogglerViewModel<LocationServiceState>(collection.LocationService, dispatcher);
            
            PackageArranger = new ServiceTogglerViewModel<GenericState>(collection.PackageArranger, dispatcher);
            
            TransferService = new ServiceTogglerViewModel<GenericState>(collection.TransferService, dispatcher);
            
            PackageBuffer = new PackageBufferViewModel(collection.PackageBuilder, settings.PackageBuffer, dispatcher);

            PackageManager = new PackageManagerViewModel(collection.PackageManager, PackageArranger, dispatcher);

            TransferSetup = new TransferServiceViewModel(collection.TransferService, TransferService, settings.TransferService, dispatcher);
            This = this;
        }

        public TransferServiceViewModel TransferSetup { get; }

        public PackageBufferViewModel PackageBuffer { get; }

        public ControlPanelViewModel This { get; }

        public IServiceTogglerViewModel LocationService { get; }

        public IServiceTogglerViewModel PackageArranger { get; }

        public IServiceTogglerViewModel TransferService { get; }

        public PackageManagerViewModel PackageManager { get; }

        public bool IsPipelineRunning
        {
            get => _isPipelineRunning;
            private set
            {
                if (value == _isPipelineRunning) return;
                if (value)
                {
                    var _ = _pipeline.Start();
                }
                else
                {
                    var _ = _pipeline.Stop();
                }
            }
        }
    }
}