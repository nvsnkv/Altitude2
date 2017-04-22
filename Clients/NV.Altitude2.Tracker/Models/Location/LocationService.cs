using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using NV.Altitude2.Tracker.Models.Pipeline;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Location
{
    internal sealed class LocationService : OutPipelineService<Measurement, LocationServiceState>
    {
        private static readonly IReadOnlyDictionary<PositionStatus, LocationServiceState> StateMap = new Dictionary<PositionStatus, LocationServiceState>()
        {
            { PositionStatus.Disabled, LocationServiceState.Disabled },
            { PositionStatus.Initializing, LocationServiceState.Initializing },
            { PositionStatus.NoData, LocationServiceState.NoData },
            { PositionStatus.NotAvailable, LocationServiceState.Disabled },
            { PositionStatus.NotInitialized, LocationServiceState.Initializing },
            { PositionStatus.Ready, LocationServiceState.Running }
        };

        private Geolocator _locator;

        protected override Task<bool> DoInitialize()
        {
            _locator = new Geolocator
            {
                ReportInterval = 1000,
                DesiredAccuracy = PositionAccuracy.High
            };

            _locator.StatusChanged += HandleStatusChanged;
            return Task.FromResult(true);
        }

        protected override async Task DoStart()
        {
            _locator.PositionChanged += HandlePositionChanged;
            await _locator.GetGeopositionAsync();
        }

        protected override Task DoStop()
        {
            _locator.PositionChanged -= HandlePositionChanged;
            _locator.StatusChanged -= HandleStatusChanged;
            _locator = null;

            State = LocationServiceState.Disabled;
            return Task.CompletedTask;
        }

        internal event EventHandler<LocationChangedEventArgs> LocationChanged;

        private void HandlePositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var geoPoint = args.Position?.Coordinate?.Point;
            if (geoPoint == null)
            {
                return;
            }

            var pos = geoPoint.Position;

            var point = new Point((decimal)pos.Latitude, (decimal)pos.Longitude, (decimal)pos.Altitude);

            var horizontalAccuracy = (decimal)args.Position.Coordinate.Accuracy;
            var verticalAccuracy = args.Position.Coordinate.AltitudeAccuracy.HasValue
                ? (decimal)args.Position.Coordinate.AltitudeAccuracy.Value
                : decimal.MaxValue;

            var accuracy = new Accuracy(horizontalAccuracy, verticalAccuracy);


            var measurement = new Measurement(point, accuracy, args.Position.Coordinate.Timestamp.LocalDateTime);
            RaiseLocationChanged(measurement);
        }

        private void HandleStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            State = StateMap[args.Status];
        }

        private void RaiseLocationChanged(Measurement measurement)
        {
            LocationChanged?.Invoke(this, new LocationChangedEventArgs(measurement));
        }
    }
}