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
        private string _packagesFolder;

        internal ControlPanelViewModel(ServicesCollection collection, CoreDispatcher dispatcher):base(dispatcher)
        {
            _collection = collection;
            _settings = collection.ApplicationSettings.Current;

            LocationService = new ServiceTogglerViewModel<LocationServiceState>(collection.LocationService, dispatcher);
            LocationService.PropertyChanged += (o, e) => _settings.Services.LocationEnabled = LocationService.IsEnabled;

            PackageArranger = new ServiceTogglerViewModel<GenericState>(collection.PackageArranger, dispatcher);
            PackageArranger.PropertyChanged += (o, e) => _settings.Services.PackagingEnabled = PackageArranger.IsEnabled;

            PackageBuffer = new PackageBufferViewModel(collection.PackageBuilder, _settings.PackageBuffer, dispatcher);

            _collection.PackageManager.CollectionChanged += async (o, e) => await Dispatch(() => PackagesFolder = GetPackagesFolder());
            This = this;

            ApplySettings();
        }

        private string GetPackagesFolder()
        {
            return _collection.PackageManager.FolderPath ?? "Package manager is not initialized";
        }

        public PackageBufferViewModel PackageBuffer { get; }

        public ControlPanelViewModel This { get; }

        public IServiceTogglerViewModel LocationService { get; }

        public IServiceTogglerViewModel PackageArranger { get; }

        public string PackagesFolder
        {
            get => _packagesFolder;
            private set
            {
                if (value == _packagesFolder) return;
                _packagesFolder = value;
                RaisePropertyChanged();
            }
        }

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