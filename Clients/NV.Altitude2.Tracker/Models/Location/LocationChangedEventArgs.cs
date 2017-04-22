using System;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Annotations;

namespace NV.Altitude2.Tracker.Models.Location
{
    internal class LocationChangedEventArgs
    {
        public LocationChangedEventArgs([NotNull] Measurement measurement)
        {
            Measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));
        }

        internal Measurement Measurement { get; }
    }
}