using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NV.Altitude2.ApiServer.Helpers;
using NV.Altitude2.ApiServer.Models;
using NV.Altitude2.Domain;

namespace NV.Altitude2.ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class MeasurementsController : Controller
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        private readonly MeasurementsContext _context;

        public MeasurementsController(MeasurementsContext context)
        {
            this._context = context;
        }

        [HttpGet("count")]
        public async Task<long> Count()
        {
            return await _context.Measurements.LongCountAsync();
        }

        [HttpPut]
        public async Task Add()
        {
            if (!Guid.TryParse(Request.Headers["X-DEVICE-ID"], out Guid deviceId))
            {
                deviceId = Guid.NewGuid();
                Response.Headers.Remove("X-DEVICE-ID");
                Response.Headers.Append("X-DEVICE-ID", deviceId.ToString());
            } 

            IList<Measurement> measurements;
            using (var stream = new DeflateStream(Request.Body, CompressionMode.Decompress))
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                measurements = _serializer.Deserialize<List<Measurement>>(jsonReader);
            }

            await _context.Measurements.AddRangeAsync(measurements.Select(m =>
            {
                var db = (DbMeasurement) m;
                db.DeviceId = deviceId;
                return db;
            }));
            await _context.SaveChangesAsync();
        }

        [HttpGet("{count}")]
        public FileResult GetGpx(int count)
        {

            var measurements = _context.Measurements.Where(m => m.Altitude < 10000).Take(count).OrderBy(m => m.Timestamp);
            return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outStream, _) =>
            {
                var nfi = new NumberFormatInfo
                {
                    NumberDecimalSeparator = ".",
                    CurrencyDecimalSeparator = "."
                };

                using(var iterator = measurements.GetEnumerator())
                using (var zipFile = new ZipArchive(new WriteOnlyStreamWrapper(outStream), ZipArchiveMode.Create))
                {
                    var chunk = 1;

                    var hasData = iterator.MoveNext();
                    while (hasData)
                    {
                        var items = 0;
                        var zipEntry = zipFile.CreateEntry($"Track{chunk}.gpx");
                        using (var zipStream = zipEntry.Open())
                        {
                            using (var writer = XmlWriter.Create(zipStream, new XmlWriterSettings {Indent = true, Async = true, Encoding = Encoding.ASCII, NewLineChars = "\n"}))
                            {
                                await writer.StartTrack($"Track {chunk}", iterator.Current.Timestamp);
                                
                                do
                                {
                                    var current = iterator.Current;
                                    DbMeasurement next = null;
                                    while (next == null && iterator.MoveNext())
                                    {
                                        next = iterator.Current;
                                        if (current.Latitude == next.Latitude && current.Longitude == next.Longitude)
                                        {
                                            if (current.HorizontalAccuracy >= next.HorizontalAccuracy
                                                && current.VerticalAccuracy >= next.VerticalAccuracy)
                                            {
                                                next = current;
                                            }

                                            next = null;
                                        }
                                    }

                                    if (current != null)
                                    {
                                        await writer.WritePoint(nfi, (Measurement)current);
                                        items++;
                                    }

                                    hasData = next != null;
                                } while (hasData && items < 100);

                                await writer.EndTrack();
                                await writer.FlushAsync();
                                await zipStream.FlushAsync();
                            }

                            chunk++;
                        }
                    }
                }
            })
            {
                FileDownloadName = $"altitudeTracks_{Guid.NewGuid()}.zip"
            };
        }

        
    }
}
