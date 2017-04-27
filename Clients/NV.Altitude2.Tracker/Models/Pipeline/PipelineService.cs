using System;
using System.Threading;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class PipelineService
    {
        private PipelineService _successor;

        protected PipelineService() { }

        public PipelineService Concat(PipelineService service)
        {
            return _successor = service ?? throw new ArgumentNullException(nameof(service)); ;
        }

        private async Task Receive(PipelineData data)
        {
            try
            {
                await HandleData(data);
            }      
            catch (Exception e)
            {
                RaiseErrorOccured(e);
            }
        }

        protected abstract Task HandleData(PipelineData data);

        protected virtual async Task SendNext(PipelineData data)
        {
            var receive = _successor?.Receive(data);
            if (receive != null) { await receive; }
        }

        internal event EventHandler<ServiceErrorEventArgs> ErrorOccured;

        protected virtual void RaiseErrorOccured(Exception e)
        {
            ErrorOccured?.Invoke(this, new ServiceErrorEventArgs(e));
        }
    }
}