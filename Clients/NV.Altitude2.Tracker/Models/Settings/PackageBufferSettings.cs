namespace NV.Altitude2.Tracker.Models.Settings
{
    internal class PackageBufferSettings
    {
        public decimal HorizontalAccuracy { get; set; }
        public decimal VerticalAccuracy { get; set; }
        public int PackageSize { get; set; }
    }

    internal class PackageManagerSettigns
    {
        public string FolderPath { get; set; }
    }
}