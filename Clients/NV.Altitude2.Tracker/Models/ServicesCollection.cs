using System.Collections.Generic;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Pipeline;
using NV.Altitude2.Tracker.Models.Settings;

namespace NV.Altitude2.Tracker.Models
{
    internal class ServicesCollection
    {
        internal ServicesCollection()
        {
            LocationService = new LocationService();
            ApplicationSettings = new ApplicationSettings();
        }

        internal LocationService LocationService { get; }

        internal ApplicationSettings ApplicationSettings { get; }

        internal IEnumerable<PipelineService> GetPipline()
        {
            yield return LocationService;
        }
    }
}