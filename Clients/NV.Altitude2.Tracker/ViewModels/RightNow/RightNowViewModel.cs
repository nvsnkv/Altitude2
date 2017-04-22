using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;

namespace NV.Altitude2.Tracker.ViewModels.RightNow
{
    internal class RightNowViewModel : ViewModelBase
    {
        public RightNowViewModel(ServicesCollection collection, CoreDispatcher dispatcher) : base(dispatcher)
        {
            Position = new PositionViewModel(collection.LocationService, dispatcher);
        }

        public PositionViewModel Position { get; }
    }
}