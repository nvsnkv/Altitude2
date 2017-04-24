using Windows.UI.Core;
using NV.Altitude2.Tracker.Models.Packaging;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class PackageManagerViewModel : ViewModelBase
    {
        private readonly PackageManager _manager;
        private string _packagesFolder;

        public PackageManagerViewModel(PackageManager packageManager, CoreDispatcher dispatcher) : base(dispatcher)
        {
            _manager = packageManager;
            
            _packagesFolder = GetPackagesFolder();
            packageManager.CollectionChanged += async (o, e) => await Dispatch(() => PackagesFolder = GetPackagesFolder());
        }

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

        private string GetPackagesFolder()
        {
            return _manager.FolderPath ?? "Package manager is not initialized";
        }
    }
}