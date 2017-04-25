using System;
using Windows.Storage;
using Newtonsoft.Json;

namespace NV.Altitude2.Tracker.Models.Settings
{
    internal class ApplicationSettings
    {
        private static AppSettings Default { get; } = new AppSettings()
        {
            Services = new AppSettings.ServiceStates()
            {
                LocationEnabled = false,
            },
            PackageBuffer = new PackageBufferSettings()
            {
                HorizontalAccuracy = 3m,
                VerticalAccuracy = 3m,

                PackageSize = 100
            },
            PackageManager = new PackageManagerSettigns()
            {
                FolderPath = null
            },
            TransferService = new TransferServiceSettings()
            {
                ApiUrl = null
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
                    var result = JsonConvert.DeserializeObject<AppSettings>(serialized);
                    if (result.Services != null
                        && result.PackageBuffer != null
                        && result.PackageManager != null
                        && result.TransferService != null)
                    {
                        Current = result;
                    }
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
}