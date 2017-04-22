using Windows.UI.Core;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.ViewModels.ControlPanel;
using NV.Altitude2.Tracker.ViewModels.RightNow;

namespace NV.Altitude2.Tracker.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel(ServicesCollection collection, CoreDispatcher dispatcher)
        {
            ControlPanel = new ControlPanelViewModel(collection, dispatcher);
            RightNow = new RightNowViewModel(collection, dispatcher);
        }

        public ControlPanelViewModel ControlPanel { get; }

        public RightNowViewModel RightNow { get; }
    }
}