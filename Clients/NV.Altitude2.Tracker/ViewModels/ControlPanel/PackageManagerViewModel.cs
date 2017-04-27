using System;
using System.Collections.Specialized;
using System.Windows.Input;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models.Packaging;
using NV.Altitude2.Tracker.Models.Settings;

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
        
            ClearFolder = new ClearFolderCommand(_manager, packageArranger);
            SelectFolder = new SelectFolderCommand(_manager, packageArranger);
            _packagesFolder = GetPackagesFolder();
            packageManager.CollectionChanged += async (o, e) =>
            {
                if (e.Action.Equals(NotifyCollectionChangedAction.Reset))
                {
                    await Dispatch(() => PackagesFolder = GetPackagesFolder());
                }
            };
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

        public ICommand SelectFolder { get; }

        private string GetPackagesFolder()
        {
            return _manager.IsInitialized ? _manager.FolderPath ?? "Local cache folder" : "Uninitialized";
        }
    }

    internal class SelectFolderCommand : ICommand
    {
        private readonly PackageManager _manager;
        private readonly IServiceTogglerViewModel _packageArranger;

        public SelectFolderCommand(PackageManager manager, IServiceTogglerViewModel packageArranger)
        {
            _manager = manager;
            _packageArranger = packageArranger;
            _packageArranger.PropertyChanged += (o, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return !_packageArranger.IsEnabled;
        }

        public void Execute(object parameter)
        {
            var _ = _manager.SetFolder();
        }

        public event EventHandler CanExecuteChanged;
    }

    internal class ClearFolderCommand : ICommand
    {
        private readonly PackageManager _manager;
        private readonly IServiceTogglerViewModel _packageArranger;

        public ClearFolderCommand(PackageManager manager, IServiceTogglerViewModel packageArranger)
        {
            _manager = manager;
            _packageArranger = packageArranger;
            _packageArranger.PropertyChanged += (o, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _packageArranger.IsEnabled;
        }

        public async void Execute(object parameter)
        {
            await _manager.Clear();
        }

        public event EventHandler CanExecuteChanged;
    }
}