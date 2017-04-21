using System;

namespace NV.Altitude2.Domain
{
    public sealed class Measurement
    {
        public Point Point { get; }

        public Accuracy Accuracy { get; }

        public DateTime Timestamp { get; set; }

        public Measurement(Point point, Accuracy accuracy, DateTime timestamp)
        {
            Point = point;
            Accuracy = accuracy;
            Timestamp = timestamp;
        }
    }
}