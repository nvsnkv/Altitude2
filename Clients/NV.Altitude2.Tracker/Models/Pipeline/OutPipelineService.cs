using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class OutPipelineService<TOut, TS> : StatefulPipelineService<TS> where TS : struct
    {
        protected async Task SendNext(TOut data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            await SendNext(new PipelineData(typeof(TOut), data));
        }

        protected override Task<PipelineData> HandleData(PipelineData data)
        {
            throw new NotImplementedException();
        }
    }
}