using System.Collections.Generic;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models
{
    internal class ServicesCollection
    {
        internal ServicesCollection()
        {
            LocationService = new LocationService();
        }

        internal LocationService LocationService { get; }

        internal IEnumerable<PipelineService> Get()
        {
            yield return LocationService;
        }
    }
}