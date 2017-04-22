using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal sealed class PackageBuilder : InOutPipelineService<Measurement, List<Measurement>, GenericState>
    {
        private readonly IList<Measurement> _measurements = new List<Measurement>();
        private readonly object _listLock = new object();
        private volatile bool _enabled;
        private volatile Accuracy _desiredAccuracy;

        public Accuracy DesiredAccuracy
        {
            get => _desiredAccuracy;
            set => _desiredAccuracy = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override Task<bool> DoInitialize()
        {
            return Task.FromResult(true);
        }

        protected override Task DoStart()
        {
            _enabled = true;
            State = GenericState.Enabled;

            return Task.CompletedTask;
        }

        protected override Task DoStop()
        {
            _enabled = true;
            State = GenericState.Enabled;

            return Task.CompletedTask;
        }

        protected override Task<List<Measurement>> Handle(Measurement data)
        {
            if (DesiredAccuracy.IsLessThan(data.Accuracy)) { return Task.FromResult<List<Measurement>>(null); }
        }
    }
}