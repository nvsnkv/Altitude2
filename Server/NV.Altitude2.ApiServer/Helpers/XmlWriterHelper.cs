using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using NV.Altitude2.Domain;

namespace NV.Altitude2.ApiServer.Helpers
{
    internal static class XmlWriterHelper
    {
        public static async Task WritePoint(this XmlWriter writer, NumberFormatInfo nfi, Measurement measurement)
        {
            var point = measurement.Point;

            await writer.WriteStartElementAsync(null, "trkpt", null);

            await writer.WriteAttributeStringAsync(null, "lat", null, point.Latitude.ToString("F6", nfi));
            await writer.WriteAttributeStringAsync(null, "lon", null, point.Longitude.ToString("F6", nfi));

            await writer.WriteStartElementAsync(null, "ele", null);
            await writer.WriteStringAsync(point.Altitude.ToString("F6", nfi));
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "time", null);
            await writer.WriteStringAsync(measurement.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            await writer.WriteEndElementAsync();

            await writer.WriteEndElementAsync();
        }

        public static async Task EndTrack(this XmlWriter writer)
        {
            await writer.WriteEndElementAsync();
            await writer.WriteEndElementAsync();

            await writer.WriteEndElementAsync();
            await writer.WriteEndDocumentAsync();
        }

        public static async Task StartTrack(this XmlWriter writer, string name, DateTime time)
        {
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "gpx", null);
            await writer.WriteAttributeStringAsync(null, "version", null, "1.0");
            await writer.WriteAttributeStringAsync(null, "creator", null, "Altitude2 server");

            await writer.WriteStartElementAsync(null, "trk", null);

            await writer.WriteStartElementAsync(null, "name", null);
            await writer.WriteStringAsync(name);
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "time", null);
            await writer.WriteStringAsync(time.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "trkseg", null);
        }
    }
}