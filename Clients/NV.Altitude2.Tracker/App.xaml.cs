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
        private readonly ServicesCollection _services = new ServicesCollection();
        private readonly Pipeline _pipeline;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            _pipeline = new Pipeline(_services.GetPipeline(), PipelineErrorOccured);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var page = Window.Current.Content as MainPage;
           
            if (page == null)
            {
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                page = new MainPage
                {
                    DataContext = new MainViewModel(_pipeline, _services, Window.Current.Dispatcher)
                };
                Window.Current.Content = page;
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private static void PipelineErrorOccured(object sender, ServiceErrorEventArgs e)
        {
            Debug.WriteLine($"Error: {e.Error.Message}{Environment.NewLine}{e.Error}");
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await _pipeline.Stop();
            deferral.Complete();
        }
    }
}
