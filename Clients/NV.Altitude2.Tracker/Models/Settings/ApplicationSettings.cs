using System;
using Windows.Storage;
using Newtonsoft.Json;

namespace NV.Altitude2.Tracker.Models.Settings
{
    internal class ApplicationSettings
    {
        internal static AppSettings Default { get; } = new AppSettings()
        {
            Services = new AppSettings.ServiceStates()
            {
                LocationEnabled = false
            }
        };

        internal AppSettings Current { get; private set; } = Default;

        internal void Load ()
        {
            var serialized = ApplicationData.Current.LocalSettings.Values["settings"] as string;
            if (!string.IsNullOrEmpty(serialized))
            {
                try
                {
                    Current = JsonConvert.DeserializeObject<AppSettings>(serialized);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        internal void Save()
        {
            var serialized = JsonConvert.SerializeObject(Current);
            ApplicationData.Current.LocalSettings.Values["settings"] = serialized;
        }
    }

    internal class AppSettings
    {
        public ServiceStates Services { get; set; }

        internal class ServiceStates
        {
            public bool LocationEnabled { get; set; }
        }
    }
}