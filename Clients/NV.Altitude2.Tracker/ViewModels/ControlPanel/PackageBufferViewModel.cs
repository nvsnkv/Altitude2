using System;
using Windows.Devices.PointOfService;
using Windows.UI.Core;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Packaging;
using NV.Altitude2.Tracker.Models.Settings;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class PackageBufferViewModel : ViewModelBase
    {
        private readonly PackageBuilder _packageBuilder;
        private readonly PackageBufferSettings _settings;

        public PackageBufferViewModel(PackageBuilder packageBuilder, PackageBufferSettings settings, CoreDispatcher dispatcher) : base(dispatcher)
        {
            _packageBuilder = packageBuilder;
            _settings = settings;

            ApplySettings(true, true);
        }

        public double Horizontal
        {
            get => (double) _settings.HorizontalAccuracy;
            set
            {
                var val = (decimal) value;
                if (val == _settings.HorizontalAccuracy) return;

                _settings.HorizontalAccuracy = (decimal) value;
                ApplySettings(true, false);

                RaisePropertyChanged();
            }
        }

        public double Vertical
        {
            get => (double)_settings.VerticalAccuracy;
            set
            {
                var val = (decimal)value;
                if (val == _settings.VerticalAccuracy) return;

                _settings.VerticalAccuracy = (decimal)value;
                ApplySettings(true, false);

                RaisePropertyChanged();
            }
        }

        public double PackageSize
        {
            get => _settings.PackageSize;
            set
            {
                var val = (int)Math.Floor(value);
                if (_settings.PackageSize == val) return;

                _settings.PackageSize = val;
                ApplySettings(false, true);

                RaisePropertyChanged();
            }
        }

        private void ApplySettings(bool accuracy, bool size)
        {
            if (accuracy)
            {
                _packageBuilder.DesiredAccuracy = new Accuracy(_settings.HorizontalAccuracy, _settings.VerticalAccuracy);
            }

            if (size)
            {
                _packageBuilder.PackageSize = _settings.PackageSize;
            }
        }
    }
}