using System;
using System.Threading;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class StatefulPipelineService<TS> : PipelineService where TS : struct
    {
        private readonly TS _stoppedState;
 
        private TS _state;

        private bool _initialized;

        protected StatefulPipelineService()
        {
            _stoppedState = _state = default(TS);
        }

        protected internal TS State
        {
            get => _state;
            protected set {
                if (_state.Equals(value)) { return; }

                _state = value;

                RaiseStateChanged(_state);
            }
        }

        internal async Task Start()
        {
            if (Token.IsCancellationRequested) return;

            if (!_initialized)
            {
                await Initialize();
            }
            
            await DoStart();
        }

        internal async Task Stop()
        {
            if (Token.IsCancellationRequested) return;

            if (State.Equals(_stoppedState)) { return; }
            try
            {
                await DoStop();
            }
            catch(Exception e)
            {
                RaiseErrorOccured(e);
            }
        }

        private async Task Initialize()
        {
            if (_initialized) { return; }
            if (Token == CancellationToken.None) { return; }
            try
            {
                _initialized = await DoInitialize();
            }
            catch(Exception e)
            {
                RaiseErrorOccured(e);
                _initialized = false;
            }        
        }

        protected abstract Task<bool> DoInitialize();

        protected abstract Task DoStart();

        protected abstract Task DoStop();

        internal event EventHandler<ServiceStateChangedEventArgs<TS>> StateChanged;

        protected virtual void RaiseStateChanged (TS state)
        {
            StateChanged?.Invoke(this, new ServiceStateChangedEventArgs<TS>(state));
        }
    }
}