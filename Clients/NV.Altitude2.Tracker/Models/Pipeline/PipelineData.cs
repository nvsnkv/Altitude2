using System;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal sealed class PipelineData
    {
        public Type Type { get; }

        public object Data { get; }

        public PipelineData(Type type, object data)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}