using System.Threading.Tasks;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageCleaner:InPipelineService<string, GenericState>
    {
        private readonly PackageManager _manager;

        public PackageCleaner(PackageManager manager)
        {
            _manager = manager;
        }

        protected override async Task<bool> DoInitialize()
        {
            if (!_manager.IsInitialized)
            {
                await _manager.Initialize(Token);
            }

            return true;
        }

        protected override Task DoStart()
        {
            State = GenericState.Enabled;
            return Task.CompletedTask;
        }

        protected override Task DoStop()
        {
            State = GenericState.Disabled;
            return Task.CompletedTask;
        }

        protected override async Task Handle(string data)
        {
            await _manager.DeletePackage(data, Token);
        }
    }
}