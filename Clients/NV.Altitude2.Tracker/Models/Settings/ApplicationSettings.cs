using System;
using Windows.Storage;
using Newtonsoft.Json;

namespace NV.Altitude2.Tracker.Models.Settings
{
    internal class ApplicationSettings
    {
        private static AppSettings Default { get; } = new AppSettings()
        {
            PackageBuffer = new PackageBufferSettings()
            {
                HorizontalAccuracy = 3m,
                VerticalAccuracy = 3m,

                PackageSize = 100
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
                    if (result.PackageBuffer != null
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