using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageArranger : InOutPipelineService<List<Measurement>, string, GenericState>
    {
        private readonly PackageManager _manager;

        public PackageArranger(PackageManager manager)
        {
            _manager = manager;
        }

        protected override async Task<bool> DoInitialize()
        {
            await _manager.Initialize(Token);

            var _ = Task.Run(new Action(async () =>
            {
                while (!Token.IsCancellationRequested)
                {
                    if (State.Equals(GenericState.Enabled))
                    {
                        try
                        {
                            var pacakgeName = await _manager.GetNextPackage(Token);
                            await SendNext(pacakgeName);
                        }
                        catch (Exception e)
                        {
                            RaiseErrorOccured(e);
                        }
                    }
                    await Task.Delay(1000);
                }
            }), Token);

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

        protected override async Task Handle(List<Measurement> data)
        {
            await _manager.CreatePackage(data);
        }
    }
}