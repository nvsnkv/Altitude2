using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class InOutPipelineService<TIn, TOut, TS> : OutPipelineService<TOut, TS> where TS : struct
    {
        protected override async Task DoHandleData(PipelineData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Type == typeof(TIn))
            {
                await Handle((TIn)data.Data);
            }
        }

        protected abstract Task Handle(TIn data);
    }
}