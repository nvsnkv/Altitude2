using System;
using System.Threading;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class PipelineService
    {
        private PipelineService _successor;

        protected CancellationToken Token { get; private set; }

        protected PipelineService() { }

        internal PipelineService Concat(PipelineService service)
        {
            return _successor = service ?? throw new ArgumentNullException(nameof(service)); ;
        }

        internal async Task Receive(PipelineData data)
        {
            var result = data;
            try
            {
                result = await HandleData(data) ?? result;
            }      
            catch (Exception e)
            {
                RaiseErrorOccured(e);
            }

            if (result != null)
            {
                await SendNext(result);
            }
        }

        protected abstract Task<PipelineData> HandleData(PipelineData data);

        protected async Task SendNext(PipelineData data)
        {
            if (Token.IsCancellationRequested) return;

            var receive = _successor?.Receive(data);
            if (receive != null) { await receive; }
        }
        internal void SetCancellationToken(CancellationToken token)
        {
            Token = token;
        }

        internal event EventHandler<ServiceErrorEventArgs> ErrorOccured;

        protected virtual void RaiseErrorOccured(Exception e)
        {
            ErrorOccured?.Invoke(this, new ServiceErrorEventArgs(e));
        }
    }
}