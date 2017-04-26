using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    }
}
