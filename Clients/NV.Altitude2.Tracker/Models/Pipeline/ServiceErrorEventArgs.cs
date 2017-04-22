using System;

namespace NV.Altitude2.Tracker.Models.Pipeline
{
    internal sealed class ServiceErrorEventArgs : EventArgs
    {
        internal Exception Error { get; }

        internal ServiceErrorEventArgs(Exception e)
        {
            Error = e ?? throw new ArgumentNullException(nameof(e));
        }
    }
}