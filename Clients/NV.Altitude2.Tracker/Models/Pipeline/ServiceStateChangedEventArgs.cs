using System;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal sealed class ServiceStateChangedEventArgs<TS> : EventArgs where TS : struct
    {
        internal TS State { get; }

        internal ServiceStateChangedEventArgs(TS state)
        {
            State = state;
        }
    }
}