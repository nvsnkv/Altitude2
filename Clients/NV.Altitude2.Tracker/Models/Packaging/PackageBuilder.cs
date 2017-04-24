using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal sealed class PackageBuilder : InOutPipelineService<Measurement, List<Measurement>, GenericState>, INotifyCollectionChanged
    {
        private readonly IList<Measurement> _measurements = new List<Measurement>();
        private readonly object _listLock = new object();
        private volatile Accuracy _desiredAccuracy;
        private volatile int _packageSize;

        public PackageBuilder()
        {
            DesiredAccuracy = new Accuracy(3,3);
            PackageSize = 100;
            State = GenericState.Enabled;
        }

        public Accuracy DesiredAccuracy
        {
            get => _desiredAccuracy;
            set => _desiredAccuracy = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int PackageSize
        {
            get => _packageSize;
            set => _packageSize = value > 0 ? value : throw new ArgumentException("PackageSize should be positive!", nameof(value));
        }

        public int MeasurementsCount => _measurements.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override Task<bool> DoInitialize()
        {
            return Task.FromResult(true);
        }

        protected override Task DoStart()
        {
            return Task.CompletedTask;
        }

        protected override Task DoStop()
        {
            return Task.CompletedTask;
        }

        protected override async Task Handle(Measurement data)
        {
            if (DesiredAccuracy.IsLessThan(data.Accuracy))
            {
                return;
            }

            List<Measurement> package = null;
            lock (_listLock)
            {
                _measurements.Add(data);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[]{ data }));
                if (_measurements.Count >= _packageSize)
                {
                    package = new List<Measurement>(_measurements);
                    _measurements.Clear();
                    RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }

            if (package != null)
            {
                await SendNext(package);
            }
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}