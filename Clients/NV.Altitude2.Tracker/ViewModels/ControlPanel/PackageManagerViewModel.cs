using System;
using System.Windows.Input;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models.Packaging;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class PackageManagerViewModel : ViewModelBase
    {
        private readonly PackageManager _manager;
        private string _packagesFolder;

        public PackageManagerViewModel(PackageManager packageManager, IServiceTogglerViewModel packageArranger, CoreDispatcher dispatcher) : base(dispatcher)
        {
            PackageArranger = packageArranger;
            _manager = packageManager;
            ClearFolder = new ClearFolderCommand(_manager);
            _packagesFolder = GetPackagesFolder();
            packageManager.Initilalized += async (o, e) => await Dispatch(() => PackagesFolder = GetPackagesFolder());
        }

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

        public ICommand ClearFolder { get; }

        private string GetPackagesFolder()
        {
            return _manager.FolderPath ?? "Package manager is not initialized";
        }
    }

    internal class ClearFolderCommand : ICommand
    {
        private readonly PackageManager _manager;

        public ClearFolderCommand(PackageManager manager)
        {
            _manager = manager;
            _manager.Initilalized += (o, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _manager.IsInitialized;
        }

        public async void Execute(object parameter)
        {
            await _manager.Clear();
        }

        public event EventHandler CanExecuteChanged;
    }
}