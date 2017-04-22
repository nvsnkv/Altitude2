using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Location;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class ControlPanelViewModel
    {
        internal ControlPanelViewModel(ServicesCollection collection, CoreDispatcher dispatcher)
        {
            LocationService = new ServiceTogglerViewModel<LocationServiceState>(collection.LocationService, dispatcher);
            This = this;
        }

        public ControlPanelViewModel This { get; }

        public IServiceTogglerViewModel LocationService { get; }
    }
}