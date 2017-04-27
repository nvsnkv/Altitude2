using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class StatefulPipelineService<TS> : PipelineService , IStopable where TS : struct
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
            protected set
            {
                if (_state.Equals(value))
                {
                    return;
                }

                _state = value;

                RaiseStateChanged(_state);
            }
        }

        public async Task Start()
        {
            try
            {
                if (!_initialized)
                {
                    await Initialize();
                }
                await DoStart();
            }
            catch (Exception e)
            {
                RaiseErrorOccured(e);
            }
        }

        public async Task Stop()
        {
            if (State.Equals(_stoppedState))
            {
                return;
            }
            try
            {
                await DoStop();
            }
            catch (Exception e)
            {
                RaiseErrorOccured(e);
            }
        }

        private async Task Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            _initialized = await DoInitialize();
        }

        protected sealed override async Task HandleData(PipelineData data)
        {
            if (State.Equals(default(TS))) return;
            await DoHandleData(data);
        }

        protected override async Task SendNext(PipelineData data)
        {
            if (State.Equals(default(TS))) return;
            await base.SendNext(data);
        }

        protected abstract Task<bool> DoInitialize();

        protected abstract Task DoStart();

        protected abstract Task DoStop();

        protected abstract Task DoHandleData(PipelineData data);

        public event EventHandler<ServiceStateChangedEventArgs<TS>> StateChanged;

        protected virtual void RaiseStateChanged(TS state)
        {
            StateChanged?.Invoke(this, new ServiceStateChangedEventArgs<TS>(state));
        }
    }
}