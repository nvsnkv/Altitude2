using System;
using System.Collections.Generic;
using System.Threading;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal sealed class Pipeline : IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        internal Pipeline(IEnumerable<PipelineService> services, EventHandler<ServiceErrorEventArgs> taskErrorHandler = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            PipelineService current = null;
            foreach (var service in services){
                current = current?.Concat(service) ?? service;
                
                current.SetCancellationToken(_cts.Token);
                if (taskErrorHandler != null)
                {
                    current.ErrorOccured += taskErrorHandler;
                }
            }
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}