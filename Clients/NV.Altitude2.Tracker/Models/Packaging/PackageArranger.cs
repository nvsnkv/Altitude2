using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageArranger : InOutPipelineService<List<Measurement>, string, GenericState>
    {
        private readonly PackageManager _manager;
        private CancellationTokenSource _cts;
        private readonly object _ctsLock = new object();
        private Task _pollingTask;

        public PackageArranger(PackageManager manager)
        {
            _manager = manager;
        }

        protected override async Task<bool> DoInitialize()
        {
            await _manager.Initialize();
            var token = _cts.Token;
           
            return true;
        }

        protected override Task DoStart()
        {
            lock (_ctsLock)
            {
                if (_cts != null) return Task.CompletedTask;
                _cts = new CancellationTokenSource();
            }
            _cts = new CancellationTokenSource();

            var token = _cts.Token;
            _pollingTask = Task.Run(new Action(async () =>
            {
                do
                {
                    try
                    {
                        var pacakgeName = await _manager.GetNextPackage();
                        if (pacakgeName != null)
                        {
                            await SendNext(pacakgeName);
                        }
                    }
                    catch (Exception e)
                    {
                        RaiseErrorOccured(e);
                    }
                    await Task.Delay(1000, token);
                } while (!token.IsCancellationRequested);

            }), token);

            State = GenericState.Enabled;   
            return Task.CompletedTask;
        }

        protected override async Task DoStop()
        {
            CancellationTokenSource cts;
            lock (_ctsLock)
            {
                if (_cts == null) return;
                cts = _cts;
                _cts = null;
            }

            try
            {
                cts.Cancel();
                await _pollingTask;
            }
            catch (TaskCanceledException)
            {
                // expected
            }

            State = GenericState.Disabled;
        }

        protected override async Task Handle(List<Measurement> data)
        {
            await _manager.CreatePackage(data);
        }
    }
}