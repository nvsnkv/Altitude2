using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private MeasurementsContext context;

        public MeasurementsController(MeasurementsContext context)
        {
            this.context = context;
        }

        [HttpGet("count")]
        public async Task<long> Count()
        {
            return await context.Measurements.LongCountAsync();
        }

        [HttpPut]
        public async Task Add()
        {
            IList<Measurement> measurements;
            using (var reader = new StreamReader(Request.Body))
            using (var jsonReader = new JsonTextReader(reader))
            {
                measurements = _serializer.Deserialize<List<Measurement>>(jsonReader);
            }

            await context.Measurements.AddRangeAsync(measurements.Select(m => (DbMeasurement) m));
            await context.SaveChangesAsync();
        }
    }
}
