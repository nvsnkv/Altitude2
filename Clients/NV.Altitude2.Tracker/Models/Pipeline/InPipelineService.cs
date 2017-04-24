using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class InPipelineService<TIn, TS> : StatefulPipelineService<TS> where TS : struct
    {
        protected override async Task HandleData(PipelineData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Type is TIn)
            {
                await Handle((TIn)data.Data);
            }
        }

        protected abstract Task Handle(TIn data);
    }
}