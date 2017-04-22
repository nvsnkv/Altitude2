using System;
using System.Threading.Tasks;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal abstract class InOutPipelineService<TIn, TOut, TS> : StatefulPipelineService<TS> where TS : struct
    {
        protected override async Task<PipelineData> HandleData(PipelineData data) 
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (Token.IsCancellationRequested) return null;
            if (!(data.Type is TIn)) { return null; }

            var result = await Handle((TIn)data.Data);

            return result == null ? null : new PipelineData(typeof(TOut), result);
        }

        protected abstract Task<TOut> Handle(TIn data);
    }
}