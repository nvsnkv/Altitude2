using System;
using System.Diagnostics;
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

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var page = Window.Current.Content as MainPage;
            if (_session == null)
            {
                _services.ApplicationSettings.Load();
                _session = new ExtendedExecutionSession()
                {
                    Reason = ExtendedExecutionReason.LocationTracking,
                    Description = "Main app activity"
                };

                _pipeline = new Pipeline(_services.GetPipline(), PipelineErrorOccured);

                _session.Revoked += HandleSessionRevoked;
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

            var result = await _session.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    break;
                case ExtendedExecutionResult.Denied:
                    _session.Revoked -= HandleSessionRevoked;
                    _pipeline.Dispose();
                    _pipeline = null;
                    _session = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void PipelineErrorOccured(object sender, ServiceErrorEventArgs e)
        {
            Debug.WriteLine($"Error: {e.Error.Message}{Environment.NewLine}{e.Error}");
        }

        private void HandleSessionRevoked(object o, ExtendedExecutionRevokedEventArgs a)
        {
            _pipeline?.Dispose();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            _services.ApplicationSettings.Save();
        }
    }
}
