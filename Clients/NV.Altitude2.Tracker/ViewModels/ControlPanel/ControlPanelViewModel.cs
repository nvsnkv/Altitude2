using System.Threading.Tasks;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Pipeline;
using NV.Altitude2.Tracker.Models.Settings;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class ControlPanelViewModel : ViewModelBase
    {
        private readonly ServicesCollection _collection;
        private readonly AppSettings _settings;
        
        internal ControlPanelViewModel(ServicesCollection collection, CoreDispatcher dispatcher):base(dispatcher)
        {
            _collection = collection;
            _settings = collection.ApplicationSettings.Current;

            LocationService = new ServiceTogglerViewModel<LocationServiceState>(collection.LocationService, dispatcher);
            LocationService.PropertyChanged += (o, e) => _settings.Services.LocationEnabled = LocationService.IsEnabled;

            PackageArranger = new ServiceTogglerViewModel<GenericState>(collection.PackageArranger, dispatcher);
            PackageArranger.PropertyChanged += (o, e) => _settings.Services.PackagingEnabled = PackageArranger.IsEnabled;

            PackageBuffer = new PackageBufferViewModel(collection.PackageBuilder, _settings.PackageBuffer, dispatcher);

            PackageManager = new PackageManagerViewModel(collection.PackageManager, collection.ApplicationSettings.Current.PackageManager, PackageArranger, dispatcher);
            This = this;

            ApplySettings();
        }

        public PackageBufferViewModel PackageBuffer { get; }

        public ControlPanelViewModel This { get; }

        public IServiceTogglerViewModel LocationService { get; }

        public IServiceTogglerViewModel PackageArranger { get; }

        public PackageManagerViewModel PackageManager { get; }

        
        private void ApplySettings()
        {
            if (_settings.Services.LocationEnabled)
            {
                var _ = _collection.LocationService.Start();
            }

            if (_settings.Services.PackagingEnabled)
            {
                var _ = _collection.PackageArranger.Start();
            }
        }
    }
}