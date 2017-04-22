using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class InPipelineService<TIn, TS> : StatefulPipelineService<TS> where TS : struct
    {
        protected override async Task<PipelineData> HandleData(PipelineData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (Token.IsCancellationRequested) return null;

            if (data.Type is TIn)
            {
                await Handle((TIn)data.Data);
            }
            return null;
        }

        protected abstract Task Handle(TIn data);
    }
}