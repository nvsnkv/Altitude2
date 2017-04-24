using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageSaver : InOutPipelineService<List<Measurement>, string, GenericState>
    {
        protected override Task<bool> DoInitialize()
        {
            throw new NotImplementedException();
        }

        protected override Task DoStart()
        {
            State = GenericState.Enabled;   
            return Task.CompletedTask;
        }

        protected override Task DoStop()
        {
            State = GenericState.Enabled;
            return Task.CompletedTask;
        }

        protected override Task Handle(List<Measurement> data)
        {
            throw new NotImplementedException();
        }
    }
}