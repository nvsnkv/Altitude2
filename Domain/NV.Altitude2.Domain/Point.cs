using System;

namespace NV.Altitude2.Domain
{
    public sealed class Point
    {
        public decimal Latitude { get; }
        
        public decimal Longitude { get; }

        public decimal Altitude { get; }

        public Point(decimal latitude, decimal longitude, decimal altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
    }
}
