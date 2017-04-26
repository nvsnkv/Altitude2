using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NV.Altitude2.Domain;

namespace NV.Altitude2.ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class MeasurementsController : Controller
    {
        private readonly JsonSerializer _serializer = new JsonSerializer(); 

        [HttpPut]
        public void Add()
        {
            using (var reader = new StreamReader(Request.Body))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var measurements = _serializer.Deserialize<List<Measurement>>(jsonReader);
            }
        }
    }
}
