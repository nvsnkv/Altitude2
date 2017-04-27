using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal interface IStopable
    {
        Task Stop();
    }
}