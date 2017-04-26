using System;
using NV.Altitude2.Domain;

namespace NV.Altitude2.ApiServer.Models
{
    public class DbMeasurement
    {
        public long Id { get; set; }

        public Guid DeviceId { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal Altitude { get; set; }

        public decimal HorizontalAccuracy { get; set; }

        public decimal VerticalAccuracy { get; set; }

        public DateTime Timestamp { get; set; }

        public static explicit operator Measurement(DbMeasurement db)
        {
            return new Measurement(
                new Point(db.Latitude, db.Longitude, db.Altitude),
                new Accuracy(db.HorizontalAccuracy, db.VerticalAccuracy),
                db.Timestamp);
        }

        public static explicit operator DbMeasurement(Measurement m)
        {
            return new DbMeasurement()
            {
                Latitude = m.Point.Latitude,
                Longitude = m.Point.Longitude,
                Altitude = m.Point.Altitude,
                HorizontalAccuracy = m.Accuracy.Horizontal,
                VerticalAccuracy = m.Accuracy.Vertical,
                Timestamp = m.Timestamp
            };
        }
    }
}