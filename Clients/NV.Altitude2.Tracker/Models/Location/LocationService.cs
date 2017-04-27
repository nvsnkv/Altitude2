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

        private Task _fakeProvider;
        private bool _sendFakeData;

        public bool SendFakeData
        {
            get => _sendFakeData;
            set {
                if (value == _sendFakeData) return;
                _sendFakeData = value;

                if (value)
                {
                    StartFakeProvider();
                }
                else
                {
                    StopFakeProvider();
                }
            }
        }

        private void StopFakeProvider()
        {
            if (!_fakeProvider.IsCompleted)
            {
                _fakeProvider.Wait(10000);
            }
        }

        private void StartFakeProvider()
        {
            _fakeProvider = Task.Run(async () =>
            {
                do
                {
                    var measurement = new Measurement(new Point(0, 0, 10001), new Accuracy(1, 1), DateTime.Now);
                    RaiseLocationChanged(measurement);
                    await SendNext(measurement);
                    await Task.Delay(1000);
                } while (_sendFakeData);
            });
        }

        protected override Task<bool> DoInitialize()
        {
            return Task.FromResult(true);
        }

        protected override async Task DoStart()
        {
            _locator = new Geolocator
            {
                ReportInterval = 1000,
                DesiredAccuracy = PositionAccuracy.High
            };

            _locator.StatusChanged += HandleStatusChanged;
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

        private async void HandlePositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                var geoPoint = args.Position?.Coordinate?.Point;
                if (geoPoint == null)
                {
                    return;
                }

                var pos = geoPoint.Position;

                var point = new Point((decimal) pos.Latitude, (decimal) pos.Longitude, (decimal) pos.Altitude);

                var horizontalAccuracy = (decimal) args.Position.Coordinate.Accuracy;
                var verticalAccuracy = args.Position.Coordinate.AltitudeAccuracy.HasValue
                    ? (decimal) args.Position.Coordinate.AltitudeAccuracy.Value
                    : decimal.MaxValue;

                var accuracy = new Accuracy(horizontalAccuracy, verticalAccuracy);


                var measurement = new Measurement(point, accuracy, args.Position.Coordinate.Timestamp.LocalDateTime);
                RaiseLocationChanged(measurement);
                await SendNext(measurement);
            }
            catch (Exception e)
            {
                RaiseErrorOccured(e);
            }
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