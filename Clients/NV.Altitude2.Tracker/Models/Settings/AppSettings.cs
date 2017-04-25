namespace NV.Altitude2.Tracker.Models.Settings
{
    internal partial class AppSettings
    {
        public ServiceStates Services { get; set; }
        public PackageBufferSettings PackageBuffer { get; set; }
        public PackageManagerSettigns PackageManager { get; set; }
        public TransferServiceSettings TransferService { get; set; }
    }
}