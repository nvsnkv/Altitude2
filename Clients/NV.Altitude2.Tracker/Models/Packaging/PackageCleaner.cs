using System.Threading.Tasks;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageCleaner:PipelineService
    {
        private readonly PackageManager _manager;

        public PackageCleaner(PackageManager manager)
        {
            _manager = manager;
            
        }

        protected async override Task HandleData(PipelineData data)
        {
            if (data.Type != typeof(string)) return;

            if (!_manager.IsInitialized)
            {
                await _manager.Initialize();
            }

            await _manager.DeletePackage((string) data.Data);
        }
    }
}