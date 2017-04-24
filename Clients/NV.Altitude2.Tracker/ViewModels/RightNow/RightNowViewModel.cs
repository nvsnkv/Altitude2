using System;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Packaging;

namespace NV.Altitude2.Tracker.ViewModels.RightNow
{
    internal class RightNowViewModel : ViewModelBase
    {
        private string _trackerState;
        private int _measurementsCount;
        private int _packagesCount;

        public RightNowViewModel(ServicesCollection collection, CoreDispatcher dispatcher) : base(dispatcher)
        {
            Position = new PositionViewModel(collection.LocationService, dispatcher);

            collection.LocationService.StateChanged += async (o, e) => await Dispatch(() =>
            {
                switch (e.State)
                {
                    case Models.Location.LocationServiceState.Disabled:
                        TrackerState = "Disabled";
                        break;
                    case Models.Location.LocationServiceState.Initializing:
                        TrackerState = "Initializing";
                        break;
                    case Models.Location.LocationServiceState.NoData:
                        TrackerState = "No data available";
                        break;
                    case Models.Location.LocationServiceState.Running:
                        TrackerState = "Running!";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            collection.PackageBuilder.CollectionChanged += async (o, e) => await Dispatch(() =>
            {
                MeasurementsCount = collection.PackageBuilder.MeasurementsCount;
            });

            collection.PackageManager.CollectionChanged += async (o, e) => await Dispatch(() =>
            {
                PackagesCount = collection.PackageManager.PackagesCount;
            });
        }

        public PositionViewModel Position { get; }

        public string TrackerState  
        {
            get => _trackerState;
            private set
            {
                if (value == _trackerState) return;
                _trackerState = value;
                RaisePropertyChanged();
            }
        }

        public int MeasurementsCount
        {
            get => _measurementsCount;
            private set
            {
                if (value == _measurementsCount) return;
                _measurementsCount = value;
                RaisePropertyChanged();
            }
        }

        public int PackagesCount
        {
            get => _packagesCount;
            private set
            {
                if (value == _packagesCount) return;
                _packagesCount = value;
                RaisePropertyChanged();
            }
        }
    }
}