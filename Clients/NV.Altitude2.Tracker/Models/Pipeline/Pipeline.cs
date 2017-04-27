using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal sealed class Pipeline
    {
        private readonly IList<PipelineService> _services;

        private ExtendedExecutionSession _session;
        private readonly object _sessionLock = new object();
        private GenericState _state;

        internal Pipeline(IEnumerable<PipelineService> services, EventHandler<ServiceErrorEventArgs> taskErrorHandler = null)
        {
            _services = services?.ToList() ?? throw new ArgumentNullException(nameof(services));

            PipelineService current = null;
            foreach (var service in _services){
                current = current?.Concat(service) ?? service;
                
                if (taskErrorHandler != null)
                {
                    current.ErrorOccured += taskErrorHandler;
                }
            }
        }

        public GenericState State
        {
            get => _state;
            private set
            {
                if (_state.Equals(value)) return;
                _state = value;
                RaiseStateChanged(new ServiceStateChangedEventArgs<GenericState>(_state));
            }
        }

        public event EventHandler<ServiceStateChangedEventArgs<GenericState>> StateChanged;

        public async Task Start()
        {
            lock (_sessionLock)
            {
                if (_session != null) return;
                _session = new ExtendedExecutionSession()
                {
                    Reason = ExtendedExecutionReason.LocationTracking,
                    Description = "Main tracker activity"
                };
            }

            var result = await _session.RequestExtensionAsync();
            if (result != ExtendedExecutionResult.Allowed)
            {
                await Stop();
            }
            else
            {
                _session.Revoked += HandleSessionRevoked;
            }

            State = GenericState.Enabled;
        }

        public async Task Stop()
        {
            if (!ReleaseSession(out ExtendedExecutionSession session))
            {
                return;
            }

            await DoStop(session);
        }

        private async Task DoStop(ExtendedExecutionSession session)
        {
            session.Revoked -= HandleSessionRevoked;
            session.Dispose();

            foreach (var service in _services)
            {
                var stopable = service as IStopable;
                if (stopable != null)
                {
                    await stopable.Stop();
                }
            }

            State = GenericState.Disabled;
        }

        private bool ReleaseSession(out ExtendedExecutionSession session)
        {
            session = null;
            lock (_sessionLock)
            {
                if (_session != null)
                {
                    session = _session;
                    _session = null;
                }
            }

            return session != null;
        }

        private async void HandleSessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await Stop();
        }

        private void RaiseStateChanged(ServiceStateChangedEventArgs<GenericState> e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}