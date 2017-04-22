using System;
using Windows.UI.Core;
using NV.Altitude2.Tracker.Models.Location;

namespace NV.Altitude2.Tracker.ViewModels.RightNow
{
    internal class PositionViewModel : ViewModelBase
    {
        private decimal _latitude;
        private decimal _longitude;
        private decimal _altitude;
        private DateTime _timestamp;
        private decimal _horizontalAccuracy;
        private decimal _verticalAccuracy;

        public PositionViewModel(LocationService locationService, CoreDispatcher dispatcher) : base(dispatcher)
        {
            locationService.LocationChanged += HandleLocationChanged;
        }

        public decimal Latitude
        {
            get => _latitude;
            private set
            {
                if (value == _latitude) return;
                _latitude = value;
                RaisePropertyChanged();
            }
        }

        public decimal Longitude
        {
            get => _longitude;
            private set
            {
                if (value == _longitude) return;
                _longitude = value;
                RaisePropertyChanged();
            }
        }

        public decimal Altitude
        {
            get => _altitude;
            private set
            {
                if (value == _altitude) return;
                _altitude = value;
                RaisePropertyChanged();
            }
        }

        public decimal HorizontalAccuracy
        {
            get => _horizontalAccuracy;
            private set
            {
                if (value == _horizontalAccuracy) return;
                _horizontalAccuracy = value;
                RaisePropertyChanged();
            }
        }

        public decimal VerticalAccuracy
        {
            get => _verticalAccuracy;
            private set
            {
                if (value == _verticalAccuracy) return;
                _verticalAccuracy = value;
                RaisePropertyChanged();
            }
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            private set
            {
                if (value.Equals(_timestamp)) return;
                _timestamp = value;
                RaisePropertyChanged();
            }
        }

        private async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            await Dispatch(() =>
            {
                Latitude = e.Measurement.Point.Latitude;
                Longitude = e.Measurement.Point.Longitude;
                Altitude = e.Measurement.Point.Altitude;
                HorizontalAccuracy = e.Measurement.Accuracy.Horizontal;
                VerticalAccuracy = e.Measurement.Accuracy.Vertical;
                Timestamp = e.Measurement.Timestamp;
            });
        }
    }
}