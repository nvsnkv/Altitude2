using System.Threading.Tasks;
using NV.Altitude2.Tracker.Models.Pipeline;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Location
{
    internal class LocationService : OutPipelineService<Measurement, LocationServiceState>
    {
        protected override Task<bool> DoInitialize()
        {
            throw new System.NotImplementedException();
        }

        protected override Task DoStart()
        {
            throw new System.NotImplementedException();
        }

        protected override Task DoStop()
        {
            throw new System.NotImplementedException();
        }
    }
}