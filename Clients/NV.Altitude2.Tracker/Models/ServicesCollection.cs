using System.Collections.Generic;
using NV.Altitude2.Tracker.Models.Location;
using NV.Altitude2.Tracker.Models.Packaging;
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
            PackageBuilder = new PackageBuilder();

            PackageManager = new PackageManager();
            PackageArranger = new PackageArranger(PackageManager);
        }

        internal LocationService LocationService { get; }

        internal PackageBuilder PackageBuilder { get; }

        internal PackageArranger PackageArranger { get; }

        internal ApplicationSettings ApplicationSettings { get; }

        internal PackageManager PackageManager { get; }


        internal IEnumerable<PipelineService> GetPipline()
        {
            yield return LocationService;
            yield return PackageBuilder;
            yield return PackageArranger;
        }
    }
}