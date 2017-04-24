using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Settings;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class ControlPanelViewModel
    {
        private readonly ServicesCollection _collection;
        private readonly AppSettings _settings;

        internal ControlPanelViewModel(ServicesCollection collection, CoreDispatcher dispatcher)
        {
            _collection = collection;
            _settings = collection.ApplicationSettings.Current;
            LocationService = new ServiceTogglerViewModel<LocationServiceState>(collection.LocationService, dispatcher);
            LocationService.PropertyChanged += (o, e) => _settings.Services.LocationEnabled = LocationService.IsEnabled;
            PackageBuffer = new PackageBufferViewModel(collection.PackageBuilder, _settings.PackageBuffer, dispatcher);
            This = this;

            ApplySettings();
        }

        public PackageBufferViewModel PackageBuffer { get; }

        public ControlPanelViewModel This { get; }

        public IServiceTogglerViewModel LocationService { get; }

        private void ApplySettings()
        {
            if (_settings.Services.LocationEnabled)
            {
                var _ = _collection.LocationService.Start();
            }
        }
    }
}