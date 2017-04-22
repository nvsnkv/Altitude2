using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Xaml;
using NV.Altitude2.Tracker.Models;
using NV.Altitude2.Tracker.Models.Pipeline;
using NV.Altitude2.Tracker.ViewModels;

namespace NV.Altitude2.Tracker
{
    sealed partial class App : Application
    {
        private Pipeline _pipeline;
        private readonly ServicesCollection _services = new ServicesCollection();
        private ExtendedExecutionSession _session;
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var page = Window.Current.Content as MainPage;
            if (_pipeline == null)
            {
                _pipeline = new Pipeline(_services.Get());
            }
            if (page == null)
            {
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                page = new MainPage()
                {
                    DataContext = new MainViewModel(_services, Window.Current.Dispatcher)
                };
                Window.Current.Content = page;
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
